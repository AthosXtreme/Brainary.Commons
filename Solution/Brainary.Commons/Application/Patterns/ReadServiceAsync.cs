using System.Linq.Expressions;
using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Base class for async readonly business logic service on an entity
    /// </summary>
    public abstract class ReadServiceAsync<T> : IReadServiceAsync<T> where T : Entity
    {
        protected IReadOnlyRepositoryAsync<T> Repository { get; set; }

        public ReadServiceAsync(IReadOnlyRepositoryAsync<T> repository)
        {
            Repository = repository;
        }

        public virtual async Task<T?> ReadOne(object id)
        {
            return await Repository.FindById(id);
        }

        public virtual IAsyncEnumerable<T> ReadAll()
        {
            return Repository.FindAll();
        }

        public virtual IAsyncEnumerable<T> ReadMany(Expression<Func<T, bool>> func)
        {
            return Repository.Find(func);
        }

        public virtual IAsyncEnumerable<T> ReadMany(ISpecification<T> specification)
        {
            return Repository.Find(specification);
        }
    }
}
