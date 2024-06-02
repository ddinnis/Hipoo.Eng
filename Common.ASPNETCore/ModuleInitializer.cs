using Common.ASPNETCore;
using Commons.Commons;
using Microsoft.Extensions.DependencyInjection;

namespace Commons.ASPNETCore
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddScoped<IMemoryCacheHelper, MemoryCacheHelper>();
            //services.AddScoped<IDistributedCacheHelper, DistributedCacheHelper>();
        }
    }
}
