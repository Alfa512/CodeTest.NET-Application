using System;
using System.Collections.Generic;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Common.Contracts.Data
{
    public interface IDataContext : IDisposable
    {
        IUserRepository Users { get; }
        TEntity Add<TEntity>(TEntity entity) where TEntity : class, IEntity;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity;
        TEntity Delete<TEntity>(TEntity entity) where TEntity : class, IEntity;
        IEnumerable<TEntity> All<TEntity>() where TEntity : class, IEntity;
        int SaveChanges();
    }
}
