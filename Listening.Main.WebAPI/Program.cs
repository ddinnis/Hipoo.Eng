using CommonInitializer;
using Listening.Admin.WebAPI;
using Listening.Admin.WebAPI.Hubs;
using Listening.Domain;
using Listening.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "d:/temp/Listening.Main.log",
    EventBusQueueName = "Listening.Main"
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<EncodingEpisodeHelper>();
builder.Services.AddScoped<IListeningRepository, ListeningRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Listening.Main.WebAPI", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Listening.Main.WebAPI v1"));
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
