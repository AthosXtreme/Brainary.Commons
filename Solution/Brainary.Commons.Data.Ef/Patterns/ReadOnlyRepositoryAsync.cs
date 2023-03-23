using System;
using System.Linq.Expressions;
using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Entity Framework implementation of <see cref="IReadOnlyRepositoryAsync{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class ReadOnlyRepositoryAsync<T> : IReadOnlyRepositoryAsync<T> where T : Entity
    {
        protected DbContext Context { get; set; }

        public ReadOnlyRepositoryAsync(DbContext context)
        {
            Context = context;
        }

        public virtual IAsyncEnumerable<T> Find(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return query.Where(func).AsAsyncEnumerable();

        }

        public virtual IAsyncEnumerable<T> Find(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(Context.Set<T>().AsQueryable(), specification).AsAsyncEnumerable();
        }

        public virtual IAsyncEnumerable<T> FindAll(params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return query.AsAsyncEnumerable();
        }

        public virtual async Task<T?> FindById(object id, params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return await query.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public virtual async Task<T?> FindOne(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include)
        {
            var query = Context.Set<T>().AsQueryable();
            foreach (var item in include)
                query = query.Include(item);
            return await query.FirstOrDefaultAsync(func);
        }

        public virtual async Task<bool> Exists(Expression<Func<T, bool>> func)
        {
            return await Context.Set<T>().AnyAsync(func);
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
