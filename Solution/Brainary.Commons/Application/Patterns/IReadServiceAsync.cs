using System.Linq.Expressions;
using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Readonly contract for async business logic service on an entity
    /// </summary>
    public interface IReadServiceAsync<T> where T : Entity
    {
        Task<T?> ReadOne(object id);

        IQueryable<T> ReadAll();

        IQueryable<T> ReadMany(Expression<Func<T, bool>> func);

        IQueryable<T> ReadMany(ISpecification<T> specification);
    }
}
