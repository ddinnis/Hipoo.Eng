using Microsoft.Extensions.DependencyInjection;

namespace Commons.Commons
{
    public interface IModuleInitializer
    {
        public void Initialize(IServiceCollection services);
    }
}
