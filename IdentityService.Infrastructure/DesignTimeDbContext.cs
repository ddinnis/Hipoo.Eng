using CommonInitializer;
using MediatR;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace IdentityService.Infrastructure
{
    internal class DesignTimeDbContext : IDesignTimeDbContextFactory<IdDbContext>
    {
        public IdDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =  DbContextOptionsBuilderFactory.Create<IdDbContext>();
            return new IdDbContext(optionsBuilder.Options);
        }
    }
}
