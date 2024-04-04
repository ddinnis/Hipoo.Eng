using Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;

namespace MediatR
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddMyCustomMediatR(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            //return services.AddMediatR(assemblies.ToArray());
            return services.AddMediatR(cfg =>
            {
                foreach (var assembly in assemblies)
                {
                    cfg.RegisterServicesFromAssembly(assembly);
                }
            });
        }

        // 保证所有连接在同一个连接中。集成事件一定要在SaveChanges之后。
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx) 
        {
            //寻找带有领域事件的实体
            var domainEntities = ctx.ChangeTracker
                .Entries<IDomainEvents>()
                .Where(x => x.Entity.GetDomainEvents().Any());

            // 收集领域事件
            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.GetDomainEvents())
                .ToList();
            // 清除领域事件 确保事件不会被重复处理。
            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (INotification domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }

    }
}
