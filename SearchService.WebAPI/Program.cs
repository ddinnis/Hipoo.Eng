using CommonInitializer;
using Microsoft.Extensions.Options;
using Nest;
using SearchService.Domain;
using SearchService.Infrastructure;
using SearchService.WebAPI.EventHandlers;
using SearchService.WebAPI.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "d:/temp/SearchService.log",
    EventBusQueueName = "SearchService.WebAPI"
});
//builder.Services.Configure<ElasticSearchOptions>(builder.Configuration.GetSection("ElasticSearch"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SearchService.WebAPI", Version = "v1" });
});

builder.Services.AddTransient<ListeningEpisodeDeletedEventHandler>();
builder.Services.AddTransient<ListeningEpisodeUpsertEventHandler>();

//builder.Services.AddScoped<ISearchRepository, SearchRepository>();
//builder.Services.AddHttpClient();
//builder.Services.AddScoped<IElasticClient>(sp =>
//{
//    var singleNode = new Uri("http://elastic:mXhxgaPyPRcWN*sMPUwP@127.0.0.1:9200");
//    var option = sp.GetRequiredService<IOptions<ElasticSearchOptions>>();

//    var settings = new ConnectionSettings(singleNode);
//    return new ElasticClient(settings);
//});
//builder.Services.AddScoped<ISearchRepository, SearchRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SearchService.WebAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.UseForwardedHeaders();
app.Run();
