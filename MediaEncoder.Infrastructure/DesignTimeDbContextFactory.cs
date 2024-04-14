using CommonInitializer;
using Microsoft.EntityFrameworkCore.Design;


namespace MediaEncoder.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MediaEncoderDbContext>
    {
        public MediaEncoderDbContext CreateDbContext(string[] args)
        {
            var optionBuilder = DbContextOptionsBuilderFactory.Create<MediaEncoderDbContext>();
            return new MediaEncoderDbContext(optionBuilder.Options,null);
        }
    }
}
