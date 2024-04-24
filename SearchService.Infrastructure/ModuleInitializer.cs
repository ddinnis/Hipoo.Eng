using Commons.Commons;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nest;
using SearchService.WebAPI.Options;
using SearchService.Domain;

namespace SearchService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IElasticClient>(sp =>
            {
                var singleNode = new Uri("http://elastic:mXhxgaPyPRcWN*sMPUwP@127.0.0.1:9200");
                var option = sp.GetRequiredService<IOptions<ElasticSearchOptions>>();
                //var settings = new ConnectionSettings(option.Value.Url);
                var settings = new ConnectionSettings(singleNode);
                return new ElasticClient(settings);
            });
            services.AddScoped<ISearchRepository, SearchRepository>();
        }
    }
}
