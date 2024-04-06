using Commons.Commons;
using Listening.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<ListeningDomainService>();
        }
    }
}