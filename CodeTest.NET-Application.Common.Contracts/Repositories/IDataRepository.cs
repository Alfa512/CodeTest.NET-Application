using System.Collections.Generic;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Common.Contracts.Repositories
{
    public interface IDataRepository<T> where T : class, IEntity, new()
    {
        IEnumerable<T> All();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}