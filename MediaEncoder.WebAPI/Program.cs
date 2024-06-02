using Common.JWT;
using CommonInitializer;
using Listening.Admin.WebAPI;
using MediaEncoder.WebAPI;
using MediaEncoder.WebAPI.EventHandlers;
using MediaEncoder.WebAPI.Options;


var builder = WebApplication.CreateBuilder(args);

builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "d:/temp/MediaEncoder.log",
    EventBusQueueName = "MediaEncoder.WebAPI"
});
var Endpoint = builder.Configuration.GetSection("FileService:Endpoint");
builder.Services.Configure<FileServiceOptions>(Endpoint);
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MediaEncoder.WebAPI", Version = "v1" });
});

//builder.Services.AddScoped<IMediaEncoderRepository, MediaEncoderRepository>();
//builder.Services.AddScoped<IMediaEncoder, ToM4AEncoder>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<EncodingEpisodeHelper>();
// Cap
builder.Services.AddTransient<MediaEncodingCreatedHandler>();



// 后台转码服务
builder.Services.AddHostedService<EncodingBgService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c=>c.SwaggerEndpoint("/swagger/v1/swagger.json", "MediaEncoder.WebAPI v1"));
}

app.UseHttpsRedirection();
app.UseCors();
app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
