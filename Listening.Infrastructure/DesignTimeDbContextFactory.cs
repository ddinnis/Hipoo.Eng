﻿using CommonInitializer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace Listening.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ListeningDbContext>
    {
        public ListeningDbContext CreateDbContext(string[] args)
        {
            var optionBuilder = DbContextOptionsBuilderFactory.Create<ListeningDbContext>();
            return new ListeningDbContext(optionBuilder.Options,null);
        }
    }
}
