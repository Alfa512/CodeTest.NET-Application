using System.Collections.Generic;

namespace CodeTest.NET_Application.Common.Contracts.Repositories
{
    public interface IDataRepository<T> where T : class
    {
        IEnumerable<T> All();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveChanges();
    }
}