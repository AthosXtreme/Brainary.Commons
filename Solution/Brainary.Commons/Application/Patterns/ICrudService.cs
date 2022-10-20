using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// CRUD contract for business logic service
    /// </summary>
    public interface ICrudService<T> : IReadService<T> where T : Entity
    {
        void Create(T entity);

        void Update(T entity);

        void CreateOrUpdate(T entity);

        void Delete(object id);
    }
}
