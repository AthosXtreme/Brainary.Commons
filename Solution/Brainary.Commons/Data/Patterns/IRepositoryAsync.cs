using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Read/Write contract for async repository pattern
    /// </summary>
    public interface IRepositoryAsync<T> : IReadOnlyRepositoryAsync<T> where T : Entity
    {
        Task Create(T instance);

        Task Update(T instance);

        Task CreateOrUpdate(T instance);

        Task Remove(T instance);

        Task CreateRange(IEnumerable<T> instances);

        Task UpdateRange(IEnumerable<T> instances);

        Task RemoveRange(IEnumerable<T> instances);

        Task Commit();
    }
}