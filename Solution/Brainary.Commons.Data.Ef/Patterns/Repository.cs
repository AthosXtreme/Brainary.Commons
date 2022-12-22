using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Entity Framework implementation of <see cref="IRepository{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class Repository<T> : ReadOnlyRepository<T>, IRepository<T>, IReadOnlyRepository<T> where T : Entity
    {
        public Repository(DbContext context) : base(context) { }

        public virtual void Create(T instance)
        {
            var entry = Context.Entry(instance);
            if (entry.State != EntityState.Detached)
                throw new EntityAlreadyExistsException();

            var set = Context.Set<T>();
            T? attachedEntity = set.Find(instance.Id);
            if (attachedEntity != null)
                throw new EntityAlreadyExistsException();

            set.Add(instance);
        }

        public virtual void Update(T instance)
        {
            var entry = Context.Entry(instance);
            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                T? attachedEntity = set.Find(instance.Id);
                if (attachedEntity == null)
                    throw new EntityDoesNotExistsException();

                var attachedEntry = Context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(instance);
            }
        }

        public virtual void CreateOrUpdate(T instance)
        {
            Context.Set<T>().Update(instance);
        }

        public virtual void Remove(T instance)
        {
            var entry = Context.Entry(instance);
            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                T? attachedEntity = set.Find(instance.Id);
                if (attachedEntity == null)
                    throw new EntityDoesNotExistsException();

                Context.Entry(instance).State = EntityState.Unchanged;
                set.Remove(instance);
            }
        }

        public virtual void CreateRange(IEnumerable<T> instances)
        {
            Context.AddRange(instances);
        }

        public virtual void UpdateRange(IEnumerable<T> instances)
        {
            Context.UpdateRange(instances);
        }

        public virtual void RemoveRange(IEnumerable<T> instances)
        {
            Context.RemoveRange(instances);
        }

        public virtual void Commit()
        {
            Context.SaveChanges();
        }
    }
}
