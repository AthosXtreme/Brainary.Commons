using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Read/Write contract for repository pattern
    /// </summary>
    public interface IRepository<T> : IReadOnlyRepository<T> where T : Entity
    {
        void Create(T instance);

        void Update(T instance);

        void CreateOrUpdate(T instance);

        void Remove(T instance);

        void CreateRange(IEnumerable<T> instances);

        void UpdateRange(IEnumerable<T> instances);

        void RemoveRange(IEnumerable<T> instances);

        void Commit();
    }
}