using System.Linq.Expressions;
using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Readonly contract for business logic service
    /// </summary>
    public interface IReadService<T> where T : Entity
    {
        T? ReadOne(object id);

        IEnumerable<T> ReadAll();

        IEnumerable<T> ReadMany(Expression<Func<T, bool>> func);

        IEnumerable<T> ReadMany(ISpecification<T> specification);
    }
}
