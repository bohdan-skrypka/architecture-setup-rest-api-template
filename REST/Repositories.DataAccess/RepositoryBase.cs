using EFCoreProvider;
using Microsoft.EntityFrameworkCore;
using Repositories.DataContracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repositories.DataAccess
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected DatabaseContext DatabaseContext { get; set; }
        public RepositoryBase(DatabaseContext repositoryContext)
        {
            this.DatabaseContext = repositoryContext;
        }
        public IQueryable<T> FindAll()
        {
            return this.DatabaseContext.Set<T>().AsNoTracking();
        }
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.DatabaseContext.Set<T>()
                .Where(expression).AsNoTracking();
        }
        public void Create(T entity)
        {
            this.DatabaseContext.Set<T>().Add(entity);
        }
        public void Update(T entity)
        {
            this.DatabaseContext.Set<T>().Update(entity);
        }
        public void Delete(T entity)
        {
            this.DatabaseContext.Set<T>().Remove(entity);
        }
    }
}
