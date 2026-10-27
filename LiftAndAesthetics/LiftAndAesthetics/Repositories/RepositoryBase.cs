using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LiftAndAesthetics.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected LiftContext LiftContext { get; set; }

        public RepositoryBase(LiftContext liftContext)
        {
            this.LiftContext = liftContext;
        }

        public IQueryable<T> FindAll()
        {
            return this.LiftContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return this.LiftContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            this.LiftContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.LiftContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.LiftContext.Set<T>().Remove(entity);
        }
    }
}