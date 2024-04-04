using Commons.Commons;
using FileService.Domain;
using FileService.Infrastructure.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityService.Infrastructure
{
    class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddScoped<IFSRepository, FSRepository>();
            services.AddScoped<IStorageClient, SMBStorageClient > ();
            services.AddScoped<IStorageClient, MockCloudStorageClient>();
            //services.AddScoped<IStorageClient, UpYunStorageClient>();
            services.AddScoped<FSDomainService>();
            services.AddHttpClient();

        }
    }
}