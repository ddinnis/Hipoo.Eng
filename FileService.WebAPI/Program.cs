using CommonInitializer;
using FileService.Infrastructure.Service;


var builder = WebApplication.CreateBuilder(args);

builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "IdentityService.WebAPI",
    LogFilePath = "d:/temp/IdentityService.log"
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt => {
    opt.SwaggerDoc("v1", new() { Title = "IdentityService.WebAPI", Version = "v1" });
});

builder.Services
    .Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"))
    .Configure<UpYunStorageOptions>(builder.Configuration.GetSection("FileService:UpYun"));

//builder.Services.AddDbContext<FSDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileService.WebAPI v1"));
}

app.UseHttpsRedirection();

app.UseStaticFiles();
// 在调用 UseAuthorization 之前调用 UseStaticFiles
app.UseAuthorization();


app.UseCors();//启用Cors
app.UseForwardedHeaders();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
