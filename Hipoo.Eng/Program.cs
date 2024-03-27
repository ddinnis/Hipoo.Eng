using Common.JWT;
using IdentityService.Domain;
using IdentityService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using Serilog;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( opt => {
    opt.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
});

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

// 注入 JWTOptions
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

// 先验证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x =>
{
    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt.SigningKey);
    var secKey = new SymmetricSecurityKey(keyBytes);
    x.TokenValidationParameters = new()
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = secKey
    };
});
// 再授权
builder.Services.AddAuthorization();
builder.Services.AddScoped<IIdentityRepository, IdentityRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 配置时间格式
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
});

string[] urls = new[] { "http://localhost:3000" };// 前端的url
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
  .AllowAnyMethod().AllowAnyHeader().AllowCredentials())
);

builder.Services.AddLogging(builder =>
{
    Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("d:/temp/FileService.log")
    .CreateLogger();
    builder.AddSerilog();
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
