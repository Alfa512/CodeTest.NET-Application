using System;
using System.Collections.Generic;
using CodeTest.NET_Application.Common.Contracts.Repositories;

namespace CodeTest.NET_Application.Common.Contracts.Data
{
    public interface IDataContext : IDisposable
    {
        IUserRepository Users { get; }
        TEntity Add<TEntity>(TEntity entity) where TEntity : class;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        TEntity Delete<TEntity>(TEntity entity) where TEntity : class;
        IEnumerable<TEntity> All<TEntity>() where TEntity : class;
        int SaveChanges();
    }
}
