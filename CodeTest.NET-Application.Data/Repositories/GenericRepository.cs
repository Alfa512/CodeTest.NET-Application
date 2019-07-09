using System.Linq;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodeTest.NET_Application.Data.Repositories
{
    public abstract class GenericRepository<T> : IDataRepository<T> where T : class
    {
        protected readonly IDataContext Context;

        protected GenericRepository(IDataContext context)
        {
            Context = context;
        }

        public IQueryable<T> All()
        {
            return Context.Set<T>();
        }

        public void Add(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Context.Set<T>().Attach(entity);
                entry = Context.Entry(entity);
            }

            entry.State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }

        public void SaveCganges()
        {
            Context.SaveChanges();
        }

    }
}