using CommonInitializer;
using Listening.Admin.WebAPI;
using Listening.Admin.WebAPI.EventHandler;
using Listening.Admin.WebAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "d:/temp/Listening.Admin.log",
    EventBusQueueName = "Listening.Admin"
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddScoped<EncodingEpisodeHelper>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Listening.Admin.WebAPI", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddScoped<EncodingEpisodeHelper>();
builder.Services.AddSignalR();
builder.Services.AddTransient<MediaEncodingStatusChangeIntegrationHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listening.Admin.WebAPI v1"));
}
app.MapHub<EpisodeEncodingStatusHub>("/Hubs/EpisodeEncodingStatusHub");

app.UseHttpsRedirection();
app.MapControllers();

app.UseCors();//����Cors
app.UseForwardedHeaders();
//app.UseHttpsRedirection();//������ForwardedHeaders�ܺõĹ���������webapi��ĿҲû��Ҫ�������
app.UseAuthentication();
app.UseAuthorization();

app.Run();