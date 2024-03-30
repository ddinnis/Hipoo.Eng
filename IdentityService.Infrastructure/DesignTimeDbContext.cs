using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Infrastructure
{
    internal class DesignTimeDbContext : IDesignTimeDbContextFactory<IdDbContext>
    {
        public IdDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdDbContext>();
            string connStr = "Server=LAPTOP-1H2QHK1H\\SQLEXPRESS;Database=VNextDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";
            optionsBuilder.UseSqlServer(connStr);

            return new IdDbContext(optionsBuilder.Options);
        }
    }
}
