using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// CRUD contract for async business logic service on an entity
    /// </summary>
    public interface ICrudServiceAsync<T> : IReadServiceAsync<T> where T : Entity
    {
        Task Create(T entity);

        Task Update(T entity);

        Task CreateOrUpdate(T entity);

        Task Delete(object id);

        Task Delete(T entity);
    }
}
