using System;
using System.Linq.Expressions;
using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Entity Framework implementation of <see cref="IReadOnlyRepository{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class ReadOnlyRepository<T> : IReadOnlyRepository<T> where T : Entity
    {
        protected DbContext Context { get; set; }

        public ReadOnlyRepository(DbContext context)
        {
            Context = context;
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return query.Where(func);
        }

        public virtual IEnumerable<T> Find(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(Context.Set<T>().AsQueryable(), specification);
        }

        public virtual IEnumerable<T> FindAll(params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return query;
        }

        public virtual T? FindById(object id, params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return query.FirstOrDefault(obj => obj.Id == id);
        }

        public virtual T? FindOne(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return query.FirstOrDefault(func);
        }

        public virtual bool Exists(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().Any(func);
        }

        protected int ParsePage(int current, int total, int page)
        {
            if (current == 0)
                current++;
            int num = total / page + 1;
            return (current > num) ? num : current;
        }
    }
}
