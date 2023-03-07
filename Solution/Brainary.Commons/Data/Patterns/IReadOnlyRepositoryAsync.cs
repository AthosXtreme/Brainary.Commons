using System.Linq.Expressions;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Readonly contract for async repository pattern
    /// </summary>
    public interface IReadOnlyRepositoryAsync<T> where T : Entity
    {
        IAsyncEnumerable<T> Find(Expression<Func<T, bool>> func);

        IAsyncEnumerable<T> Find(ISpecification<T> specification);

        IAsyncEnumerable<T> FindAll();

        Task<T?> FindById(object id);

        Task<T?> FindOne(Expression<Func<T, bool>> func);

        Task<bool> Exists(Expression<Func<T, bool>> func);
    }
}
