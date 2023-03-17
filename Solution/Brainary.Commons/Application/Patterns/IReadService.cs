using System.Linq.Expressions;
using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Readonly contract for business logic service on an entity
    /// </summary>
    public interface IReadService<T> where T : Entity
    {
        T? ReadOne(object id);

        IEnumerable<T> ReadAll();

        IEnumerable<T> ReadMany(Expression<Func<T, bool>> func, params Expression<Func<T, object>>[] include);

        IEnumerable<T> ReadMany(ISpecification<T> specification);
    }
}
