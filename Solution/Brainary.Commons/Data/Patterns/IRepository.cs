using Brainary.Commons.Domain;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Read/Write contract for repository pattern
    /// </summary>
    public interface IRepository<T> : IReadOnlyRepository<T> where T : Entity
    {
        void Create(T instance);

        void Commit();

        void Update(T instance);

        void CreateOrUpdate(T instance);

        void Remove(T instance);
    }
}