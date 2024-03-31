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
//builder.ConfigureDbConfiguration(); 未实现
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
    // 允许对新用户应用账户锁定策略
    options.Lockout.AllowedForNewUsers = true;
    
    options.User.RequireUniqueEmail = false;

    //以下两行，把GenerateEmailConfirmationTokenAsync验证码缩短
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});

// FluentValidation
//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddFluentValidationClientsideAdapters();
//builder.Services.AddValidatorsFromAssemblyContaining<ChangeMyPasswordRequest>();

// 注入 JWTOptions
//builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

// 先验证
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
// 再授权
//builder.Services.AddAuthorization();


//builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
//builder.Services.AddScoped<IdentityDomainService>();
//builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddDataProtection();

// Identity 框架
builder.Services.AddIdentityCore<User>(options => {    //注意不是AddIdentity
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

// 配置时间格式
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
//});

//string[] urls = new[] { "http://localhost:3000" };// 前端的url
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
//    //添加安全定义
//    s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "请输入token,格式为 Bearer xxxxxxxx（注意中间必须有空格）",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    //添加安全要求
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
//    x.UseEntityFramework<IdDbContext>();  //可选项，你不需要再次配置 x.UseSqlServer 了
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
    //页面显示异常
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.WebAPI v1"));
}

app.UseHttpsRedirection();
app.UseCors();
// 会检查入站请求的指定转发头部，并根据这些头部来更新HttpContext中的相关字段，如请求方案（HttpContext.Request.Scheme）和远程IP地址（HttpContext.Connection.RemoteIpAddress）
app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
