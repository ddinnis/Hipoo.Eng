using Common.Infrastructure;
using MediaEncoder.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace MediaEncoder.Infrastructure
{
    public class MediaEncoderDbContext : BaseDbContext
    {
        public DbSet<EncodingItem> EncodingItems { get; private set; }

        public MediaEncoderDbContext(DbContextOptions<MediaEncoderDbContext> options, IMediator medaitor) : base(options, medaitor)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            // 软删除过滤
            modelBuilder.EnableSoftDeletionGlobalFilter();
        }

    }
}
