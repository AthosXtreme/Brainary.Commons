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

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().Where(func);
        }

        public virtual IEnumerable<T> Find(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(Context.Set<T>().AsQueryable(), specification);
        }

        public virtual IEnumerable<T> FindAll()
        {
            return Context.Set<T>();
        }

        public virtual T? FindById(object id)
        {
            return Context.Set<T>().FirstOrDefault(obj => obj.Id == id);
        }

        public virtual T? FindOne(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().FirstOrDefault(func);
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
