using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Base class for async CRUD business logic service on an entity
    /// </summary>
    public abstract class CrudServiceAsync<T> : ReadServiceAsync<T>, ICrudServiceAsync<T> where T : Entity
    {
        protected new IRepositoryAsync<T> Repository { get; set; }

        public CrudServiceAsync(IRepositoryAsync<T> repository)
            : base(repository)
        {
            Repository = repository;
        }

        public virtual async Task Create(T entity)
        {
            await Repository.Create(entity);
            await Repository.Commit();
        }

        public virtual async Task Update(T entity)
        {
            await Repository.Update(entity);
            await Repository.Commit();
        }

        public virtual async Task CreateOrUpdate(T entity)
        {
            await Repository.CreateOrUpdate(entity);
            await Repository.Commit();
        }

        public virtual async Task Delete(object id)
        {
            T? instance = await Repository.FindById(id);
            if (instance != null)
                await Delete(instance);
        }

        public virtual async Task Delete(T entity)
        {
            await Repository.Remove(entity);
            await Repository.Commit();
        }
    }
}