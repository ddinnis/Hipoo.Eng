using Commons.Commons;
using Microsoft.Extensions.DependencyInjection;

namespace MediaEncoder.Domain
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<MediaEncoderFactory>();
        }
    }
}
