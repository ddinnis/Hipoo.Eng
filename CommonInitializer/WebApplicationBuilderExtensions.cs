using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Common.Common;
using FluentValidation.AspNetCore;
//using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Data.SqlClient;
using Commons.Commons;
using Common.JWT;
using Commons.JWT;
using Common.EventBus;

namespace CommonInitializer
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;
            var configRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string connStr = configRoot.GetValue<string>("ConnectionString");

            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            services.RunModuleInitializers(assemblies);
            services.AddAllDbContexts(ctx =>
            {
                ctx.UseSqlServer(connStr);
            }, assemblies);

            //Authentication,Authorization
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();

            JWTOptions jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>();
            builder.Services.AddJWTAuthentication(jwtOpt);
           
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });


            ///services.AddMediatR(assemblies);

            //services.Configure<MvcOptions>(options =>
            //{
            //    options.Filters.Add<UnitOfWorkFilter>();
            //});
            //在ASP.NET Core的配置中修改JSON序列化选项，特别是关于日期时间格式的自定义
            services.Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            });

            services.AddCors(options =>
            {
                var corsOpt = configuration.GetSection("Cors").Get<CorsSettings>();
                string[] urls = corsOpt.Origins;
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            }
            );
            services.AddLogging(builder =>
            {
                Log.Logger = new LoggerConfiguration()
                   .WriteTo.Console()
                   .WriteTo.File(initOptions.LogFilePath)
                   //.WriteTo.File("d:/temp/IdentityService.log")
                   .CreateLogger();
                builder.AddSerilog();
            });
            services.AddFluentValidation(fv =>
            {
                // 多个程序集调用RegisterValidatorsFromAssemblies
                fv.RegisterValidatorsFromAssemblies(assemblies);
            });
            services.Configure<JWTOptions>(configuration.GetSection("JWT"));

            // RabbitMQ
            //var s = configuration.GetSection("RabbitMQ");
            //services.Configure<IntegrationEventRabbitMQOptions>(configuration.GetSection("RabbitMQ"));
            // services.AddEventBus(initOptions.EventBusQueueName, assemblies);

            services.AddCap(x =>
            {
                var HostName = configuration.GetSection("RabbitMQ")["HostName"];
                var ExchangeName = configuration.GetSection("RabbitMQ")["ExchangeName"];
                //x.UseEntityFramework<IdDbContext>();  //可选项，你不需要再次配置 x.UseSqlServer 了
                x.UseSqlServer(connStr);
                x.UseRabbitMQ(o =>
                {
                    o.HostName = HostName;
                    o.ExchangeName = ExchangeName;
                });
                x.UseDashboard();
            });


            //Redis的配置
            string redisConnStr = configuration.GetValue<string>("Redis:ConnStr");
            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect("localhost,password=123456");
            services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
        }
    }
}
