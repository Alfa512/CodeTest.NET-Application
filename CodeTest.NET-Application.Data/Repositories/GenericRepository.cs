using System.Collections.Generic;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Data.Repositories
{
    public abstract class GenericRepository<T> : IDataRepository<T> where T : class, IEntity, new()
    {
        protected readonly IDataContext Context;

        protected GenericRepository(IDataContext context)
        {
            Context = context;
        }

        public IEnumerable<T> All()
        {
            return Context.All<T>();
        }

        public virtual void Add(T entity)
        {
            Context.Add(entity);
        }

        public virtual IEnumerable<T> AddRange(List<T> list)
        {
            return Context.AddRange(list);
        }

        public void Update(T entity)
        {
            Context.Update(entity);
        }

        public void Delete(T entity)
        {
            Context.Delete(entity);
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }

    }
}