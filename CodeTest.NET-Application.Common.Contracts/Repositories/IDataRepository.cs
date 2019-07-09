using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodeTest.NET_Application.Common.Contracts.Repositories
{
    public interface IDataRepository<T> where T : class
    {
        IQueryable<T> All();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveCganges();
    }
}