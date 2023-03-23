using System.Linq.Expressions;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Readonly contract for async repository pattern
    /// </summary>
    public interface IReadOnlyRepositoryAsync<T> where T : Entity
    {
        IAsyncEnumerable<T> Find(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include);

        IAsyncEnumerable<T> Find(ISpecification<T> specification);

        IAsyncEnumerable<T> FindAll(params Expression<Func<T, object>>[] include);

        Task<T?> FindById(object id, params Expression<Func<T, object>>[] include);

        Task<T?> FindOne(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include);

        Task<bool> Exists(Expression<Func<T, bool>> func);
    }
}
