using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInitializer
{
    public static class DbContextOptionsBuilderFactory
    {
        //
        public static DbContextOptionsBuilder<TDbContext> Create<TDbContext>() where TDbContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
            string connStr = "Server=LAPTOP-1H2QHK1H\\SQLEXPRESS;Database=VNextDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";
            return optionsBuilder.UseSqlServer(connStr);
        }
    }
}
