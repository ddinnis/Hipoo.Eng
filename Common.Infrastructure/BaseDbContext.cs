using IdentityService.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Common.Infrastructure
{
    public class BaseDbContext: DbContext
    {
        private IMediator _medaitor;
        public BaseDbContext(DbContextOptions options, IMediator medaitor):base (options)
        {
            _medaitor = medaitor;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotImplementedException("Don not call SaveChanges, please call SaveChangesAsync instead.");
        }

        public async override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
            if (_medaitor != null) 
            {
                await _medaitor.DispatchDomainEventsAsync(this);
            }
            var softDeletedEntities = this.ChangeTracker.Entries<ISoftDelete>().Where(e => e.State == EntityState.Modified && e.Entity.IsDeleted)
                .Select(e => e.Entity).ToList();

            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            // 状态设置为 Detached。这意味着 Entity Framework 将不再跟踪这些实体的状态
            softDeletedEntities.ForEach(e => this.Entry(e).State = EntityState.Detached);

            return result;

        }
    }
}
