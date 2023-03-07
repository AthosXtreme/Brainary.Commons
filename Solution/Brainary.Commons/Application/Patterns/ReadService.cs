using System.Linq.Expressions;
using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Base class for readonly business logic service on an entity
    /// </summary>
    public abstract class ReadService<T> : IReadService<T> where T : Entity
    {
        protected IReadOnlyRepository<T> Repository { get; set; }

        public ReadService(IReadOnlyRepository<T> repository)
        {
            Repository = repository;
        }

        public virtual T? ReadOne(object id)
        {
            return Repository.FindById(id);
        }

        public virtual IEnumerable<T> ReadAll()
        {
            return Repository.FindAll();
        }

        public virtual IEnumerable<T> ReadMany(Expression<Func<T, bool>> func)
        {
            return Repository.Find(func);
        }

        public virtual IEnumerable<T> ReadMany(ISpecification<T> specification)
        {
            return Repository.Find(specification);
        }
    }
}
