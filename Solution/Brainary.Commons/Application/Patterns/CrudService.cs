using Brainary.Commons.Data.Patterns;
using Brainary.Commons.Domain;

namespace Brainary.Commons.Application.Patterns
{
    /// <summary>
    /// Base class for CRUD business logic service on an entity
    /// </summary>
    public abstract class CrudService<T> : ReadService<T>, ICrudService<T>, IReadService<T> where T : Entity
    {
        protected new IRepository<T> Repository { get; set; }

        public CrudService(IRepository<T> repository)
            : base(repository)
        {
            Repository = repository;
        }

        public virtual void Create(T entity)
        {
            Repository.Create(entity);
            Repository.Commit();
        }

        public virtual void Update(T entity)
        {
            Repository.Update(entity);
            Repository.Commit();
        }

        public virtual void CreateOrUpdate(T entity)
        {
            Repository.CreateOrUpdate(entity);
            Repository.Commit();
        }

        public virtual void Delete(object id)
        {
            T? instance = Repository.FindById(id);
            if (instance != null)
                Delete(instance);
        }

        public virtual void Delete(T entity)
        {
            Repository.Remove(entity);
            Repository.Commit();
        }
    }
}