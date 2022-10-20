using System.Linq.Expressions;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Readonly contract for repository pattern
    /// </summary>
    public interface IReadOnlyRepository<T> where T : Entity
    {
        IEnumerable<T> Find(Expression<Func<T, bool>> func);

        IEnumerable<T> Find(ISpecification<T> specification);

        T? FindById(object id);

        T? FindOne(Expression<Func<T, bool>> func);

        IEnumerable<T> FindAll();

        bool Exists(Expression<Func<T, bool>> func);
    }
}
