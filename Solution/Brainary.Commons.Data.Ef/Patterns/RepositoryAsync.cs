using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Entity Framework implementation of <see cref="IRepositoryAsync{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public abstract class RepositoryAsync<T> : ReadOnlyRepositoryAsync<T>, IRepositoryAsync<T> where T : Entity
    {
        public RepositoryAsync(DbContext context) : base(context) { }

        public virtual async Task Create(T instance)
        {
            var entry = Context.Entry(instance);
            if (entry.State != EntityState.Detached)
                throw new EntityAlreadyExistsException();

            var set = Context.Set<T>();
            T? attachedEntity = await set.FindAsync(instance.Id);
            if (attachedEntity != null)
                throw new EntityAlreadyExistsException();

            await set.AddAsync(instance);
        }

        public virtual async Task Update(T instance)
        {
            var entry = Context.Entry(instance);
            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                T? attachedEntity = await set.FindAsync(instance.Id);
                if (attachedEntity == null)
                    throw new EntityDoesNotExistsException();

                var attachedEntry = Context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(instance);
            }
        }

#pragma warning disable CS1998

        public virtual async Task CreateOrUpdate(T instance)
        {
            Context.Set<T>().Update(instance);
        }

        public virtual async Task CreateRange(IEnumerable<T> instances)
        {
            Context.AddRange(instances);
        }

        public virtual async Task UpdateRange(IEnumerable<T> instances)
        {
            Context.UpdateRange(instances);
        }

        public virtual async Task RemoveRange(IEnumerable<T> instances)
        {
            Context.RemoveRange(instances);
        }

#pragma warning restore CS1998

        public virtual async Task Remove(T instance)
        {
            var entry = Context.Entry(instance);
            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                T? attachedEntity = await set.FindAsync(instance.Id);
                if (attachedEntity == null)
                    throw new EntityDoesNotExistsException();

                Context.Entry(instance).State = EntityState.Unchanged;
                set.Remove(instance);
            }
        }

        public virtual async Task Commit()
        {
            await Context.SaveChangesAsync();
        }
    }
}
