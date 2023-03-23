using System.Linq.Expressions;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Readonly contract for repository pattern
    /// </summary>
    public interface IReadOnlyRepository<T> where T : Entity
    {
        IEnumerable<T> Find(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include);

        IEnumerable<T> Find(ISpecification<T> specification);

        IEnumerable<T> FindAll(params Expression<Func<T, object>>[] include);

        T? FindById(object id, params Expression<Func<T, object>>[] include);

        T? FindOne(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include);

        bool Exists(Expression<Func<T, bool>> func);
    }
}
