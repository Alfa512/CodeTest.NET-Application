using System;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Data.Models;
using CodeTest.NET_Application.Data.Repositories;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CodeTest.NET_Application.Data
{
    public class ApplicationContext : IdentityDbContext<User>, IDataContext
    {
        private IConfigurationService _configurationService;
        IUserRepository IDataContext.Users => new UserRepository(this);
        public ApplicationContext(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configurationService.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        void IDataContext.Commit()
        {
            SaveChanges();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}
