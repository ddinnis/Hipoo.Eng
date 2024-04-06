using Common.Infrastructure;
using Listening.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Listening.Infrastructure
{
    public class ListeningDbContext : BaseDbContext
    {
        public DbSet<Category> Categories { get; private set; }
        public DbSet<Album> Albums { get; private set; }
        public DbSet<Episode> Episodes { get; private set; }

        public ListeningDbContext(DbContextOptions options, IMediator medaitor) : base(options, medaitor)
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
