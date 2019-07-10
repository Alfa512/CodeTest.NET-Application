using System;
using System.Collections.Generic;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Common.Contracts.Data
{
    public interface IDataContext : IDisposable
    {
        IUserRepository Users { get; }
        TEntity Add<TEntity>(TEntity entity) where TEntity : class, IEntity, new();
        IEnumerable<TEntity> AddRange<TEntity>(List<TEntity> list) where TEntity : class, IEntity, new();
        TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity, new();
        TEntity Delete<TEntity>(TEntity entity) where TEntity : class, IEntity, new();
        IEnumerable<TEntity> All<TEntity>() where TEntity : class, IEntity, new();
        int SaveChanges();
    }
}
