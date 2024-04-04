using CommonInitializer;
using FileService.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityService.WebAPI;
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FSDbContext>
{
    private Mediator mediator;
    public FSDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = DbContextOptionsBuilderFactory.Create<FSDbContext>();
        return new FSDbContext(optionsBuilder.Options, mediator);
    }
}