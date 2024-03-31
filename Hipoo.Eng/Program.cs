using Common.JWT;
using IdentityService.Domain;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using IdentityService.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using FluentValidation.AspNetCore;
using FluentValidation;
using IdentityService.WebAPI.Controller.Login;
using IdentityService.Domain.Entities;
using Microsoft.OpenApi.Models;
using IdentityService.WebAPI.Event;
using Microsoft.AspNetCore.Builder;
using CommonInitializer;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.ConfigureDbConfiguration(); δʵ��
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "IdentityService.WebAPI",
    LogFilePath = "d:/temp/IdentityService.log"
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( opt => {
    opt.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
});

//IConfigurationRoot? configRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();


//var connStr = configRoot["ConnectionString"];
//builder.Services.AddDbContext<IdDbContext>(opt =>
//{
//    opt.UseSqlServer(connStr);
//});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 5;
    // ��������û�Ӧ���˻���������
    options.Lockout.AllowedForNewUsers = true;
    
    options.User.RequireUniqueEmail = false;

    //�������У���GenerateEmailConfirmationTokenAsync��֤������
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});

// FluentValidation
//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddFluentValidationClientsideAdapters();
//builder.Services.AddValidatorsFromAssemblyContaining<ChangeMyPasswordRequest>();

// ע�� JWTOptions
//builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

// ����֤
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(x =>
//{
//    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
//    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt!.SigningKey);
//    var secKey = new SymmetricSecurityKey(keyBytes);
//    x.TokenValidationParameters = new()
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = secKey
//    };
//});
// ����Ȩ
//builder.Services.AddAuthorization();


//builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
//builder.Services.AddScoped<IdentityDomainService>();
//builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDataProtection();

// Identity ���
builder.Services.AddIdentityCore<User>(options => {    //ע�ⲻ��AddIdentity
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});

var idBuilder = new IdentityBuilder(typeof(User), typeof(Role), builder.Services);
idBuilder.AddEntityFrameworkStores<IdDbContext>()
    .AddDefaultTokenProviders().AddRoleManager<RoleManager<Role>>()
    .AddUserManager<IdentityUserManager>();

// ����ʱ���ʽ
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
//});

//string[] urls = new[] { "http://localhost:3000" };// ǰ�˵�url
//builder.Services.AddCors(options =>
//    options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
//  .AllowAnyMethod().AllowAnyHeader().AllowCredentials())
//);

//builder.Services.AddLogging(builder =>
//{
//    Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .WriteTo.File("d:/temp/FileService.log")
//    .CreateLogger();
//    builder.AddSerilog();
//});


//builder.Services.AddSwaggerGen(s =>
//{
//    //��Ӱ�ȫ����
//    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "������token,��ʽΪ Bearer xxxxxxxx��ע���м�����пո�",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    //��Ӱ�ȫҪ��
//    s.AddSecurityRequirement(new OpenApiSecurityRequirement {
//        {
//            new OpenApiSecurityScheme{
//                Reference =new OpenApiReference{
//                    Type = ReferenceType.SecurityScheme,
//                    Id ="Bearer"
//                }
//            },new string[]{ }
//        }
//    });
//});
builder.Services.AddHttpClient();

// RabbitMQ
//builder.Services.AddCap(x =>
//{
//    var HostName = configRoot.GetSection("RabbitMQ")["HostName"];
//    var ExchangeName = configRoot.GetSection("RabbitMQ")["ExchangeName"];
//    x.UseEntityFramework<IdDbContext>();  //��ѡ��㲻��Ҫ�ٴ����� x.UseSqlServer ��
//    x.UseSqlServer(connStr);
//    x.UseRabbitMQ(o =>
//    {
//        o.HostName = HostName;
//        o.ExchangeName = ExchangeName;
//    });
//    x.UseDashboard();
//});
builder.Services.AddTransient<ResetPasswordEventHandler>();
builder.Services.AddTransient<UserCreatedEventHandler>();


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IEmailSender, MockEmailSender>();
    builder.Services.AddScoped<ISmsSender, MockSmsSender>();
}
else
{
    builder.Services.AddScoped<IEmailSender, SendCloudEmailSender>();
    builder.Services.AddScoped<ISmsSender, SendCloudSmsSender>();
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //ҳ����ʾ�쳣
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}

app.UseHttpsRedirection();
app.UseCors();
// ������վ�����ָ��ת��ͷ������������Щͷ��������HttpContext�е�����ֶΣ������󷽰���HttpContext.Request.Scheme����Զ��IP��ַ��HttpContext.Connection.RemoteIpAddress��
app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
