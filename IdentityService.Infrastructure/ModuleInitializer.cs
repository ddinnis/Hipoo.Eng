using Common.JWT;
using Commons.Commons;
using IdentityService.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<IdentityDomainService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}