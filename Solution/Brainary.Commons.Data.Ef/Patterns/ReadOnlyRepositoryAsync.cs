using System.Linq.Expressions;
using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;

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

        public virtual IAsyncEnumerable<T> Find(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().Where(func).AsAsyncEnumerable();
        }

        public virtual IAsyncEnumerable<T> Find(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(Context.Set<T>().AsQueryable(), specification).AsAsyncEnumerable();
        }

        public virtual IAsyncEnumerable<T> FindAll()
        {
            return Context.Set<T>().AsAsyncEnumerable();
        }

        public virtual async Task<T?> FindById(object id)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public virtual async Task<T?> FindOne(Expression<Func<T, bool>> func)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(func);
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
