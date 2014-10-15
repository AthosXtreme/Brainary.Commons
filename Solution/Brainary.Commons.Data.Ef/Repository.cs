namespace Brainary.Commons.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Transactions;
    using Domain;
    using Domain.Contracts;
    using Patterns.Specification;

    /// <summary>
    /// Entity Framework implementation of <see cref="IRepository{T}"/>
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        public Repository(DbContext context)
        {
            Context = context;
        }

        protected DbContext Context { get; set; }

        public virtual IEnumerable<T> AllMatching(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().Where(func);
        }

        public virtual IEnumerable<T> AllMatching(ISpecification<T> specification)
        {
            return Context.Set<T>().Where(specification.SatisfiedBy());
        }

        public virtual void Create(T instance)
        {
            var entry = Context.Entry(instance);

            if (entry.State != EntityState.Detached)
                throw new EntityAlreadyExistsException();

            var set = Context.Set<T>();
            var attachedEntity = set.Find(instance.Id);

            if (attachedEntity != null)
                throw new EntityAlreadyExistsException();

            Context.Set<T>().Add(instance);
            Context.SaveChanges();
        }

        public virtual void Commit()
        {
            Context.SaveChanges();
        }

        public virtual void Update(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var entry = Context.Entry(instance);

            if (entry.State == EntityState.Detached)
            {
                var set = Context.Set<T>();
                var attachedEntity = set.Find(instance.Id);

                if (attachedEntity != null)
                {
                    var attachedEntry = Context.Entry(attachedEntity);
                    attachedEntry.State = EntityState.Modified;
                    attachedEntry.CurrentValues.SetValues(instance);
                }
                else
                {
                    throw new EntityDoesNotExistsException();
                }
            }

            Context.SaveChanges();
        }

        public void CreateOrUpdate(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            Context.Set<T>().AddOrUpdate(instance);
            Context.SaveChanges();
        }

        public virtual T FindById(int id)
        {
            var entidad = Context.Set<T>().FirstOrDefault(obj => obj.Id == id);
            return entidad;
        }

        public virtual T FindOne(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().FirstOrDefault(func);
        }

        public virtual IEnumerable<T> FindAll()
        {
            return Context.Set<T>();
        }

        public virtual void Remove(T instance)
        {
            Context.Entry(instance).State = EntityState.Unchanged;
            Context.Set<T>().Remove(instance);
            Context.SaveChanges();
        }

        public virtual IResultQuery<T> Query(Expression<Func<T, bool>> func, int pageIndex, int resultPerPage)
        {
            var context = Context.Set<T>();
            var total = context.Count(func);
            var page = ParsePage(pageIndex, total, resultPerPage);
            var resultQuery = new ResultQuery<T>
            {
                Count = total,
                Page = page,
                Recordset = context.Where(func).OrderBy(x => x.Id).Skip(resultPerPage * (page - 1)).Take(resultPerPage).ToList()
            };

            return resultQuery;
        }

        public virtual IResultQuery<T> Query(ISpecification<T> specification, int pageIndex, int resultPerPage)
        {
            var context = Context.Set<T>();

            var total = context.Count(specification.SatisfiedBy());
            var page = ParsePage(pageIndex, total, resultPerPage);
            var resultQuery = new ResultQuery<T>
            {
                Count = total,
                Page = page,
                Recordset = context.Where(specification.SatisfiedBy()).OrderBy(x => x.Id).Skip(resultPerPage * (page - 1)).Take(resultPerPage).ToList()
            };

            return resultQuery;
        }

        public virtual bool Exists(Expression<Func<T, bool>> func)
        {
            return Context.Set<T>().Any(func);
        }

        public virtual void Transaction(Action<IRepository<T>> action)
        {
            using (var scope = new TransactionScope())
            {
                action(this);
                scope.Complete();
            }
        }

        protected int ParsePage(int current, int total, int page)
        {
            if (current == 0)
                current++;

            var maxPage = (total / page) + 1;
            return current > maxPage ? maxPage : current;
        }
    }
}
