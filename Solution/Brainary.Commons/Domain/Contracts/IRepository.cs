namespace Brainary.Commons.Domain.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Data;
    using Data.Patterns.Specification;

    /// <summary>
    /// Base <see cref="IEntity"/> repository interface
    /// </summary>
    /// <typeparam name="T">Concrete entity type</typeparam>
    public interface IRepository<T> where T : class, IEntity
    {
        /// <summary>
        /// Return all <typeparam name="T"></typeparam> matching the expression
        /// </summary>
        /// <param name="func">Matching expression</param>
        /// <returns>Collection</returns>
        IEnumerable<T> AllMatching(Expression<Func<T, bool>> func);

        /// <summary>
        /// Return all <typeparam name="T"></typeparam> matching the specification
        /// </summary>
        /// <param name="specification">Specification instance</param>
        /// <returns>Collection</returns>
        IEnumerable<T> AllMatching(ISpecification<T> specification);

        /// <summary>
        /// Create a new entity
        /// </summary>
        /// <param name="instance">Entity</param>
        void Create(T instance);

        /// <summary>
        /// Submit pending changes
        /// </summary>
        void Commit();

        /// <summary>
        /// Save an existing entity
        /// </summary>
        /// <param name="instance">Entity</param>
        void Update(T instance);

        /// <summary>
        /// Create if new or update existing
        /// </summary>
        /// <param name="instance">Entity</param>
        void CreateOrUpdate(T instance);

        /// <summary>
        /// Get an entity by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Entity</returns>
        T FindById(int id);

        /// <summary>
        /// Find an entity by matching expression
        /// </summary>
        /// <param name="func">Matching expression</param>
        /// <returns>Entity</returns>
        T FindOne(Expression<Func<T, bool>> func);

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>Collection</returns>
        IEnumerable<T> FindAll();

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <param name="instance"></param>
        void Remove(T instance);

        /// <summary>
        /// Return a <see cref="IResultQuery{T}"/> from matching expression
        /// </summary>
        /// <param name="func">Matching expression</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="resultPerPage">Page length</param>
        /// <returns>Result query object</returns>
        IResultQuery<T> Query(Expression<Func<T, bool>> func, int pageIndex, int resultPerPage);

        /// <summary>
        /// Return a <see cref="IResultQuery{T}"/> from specification
        /// </summary>
        /// <param name="specification">Specification instance</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="resultPerPage">Page length</param>
        /// <returns>Result query object</returns>
        IResultQuery<T> Query(ISpecification<T> specification, int pageIndex, int resultPerPage);

        /// <summary>
        /// Test if an entity exists by matching expression
        /// </summary>
        /// <param name="func">Matching expression</param>
        /// <returns>Boolean</returns>
        bool Exists(Expression<Func<T, bool>> func);

        /// <summary>
        /// Make repository action transactional
        /// </summary>
        /// <param name="action">Repository action</param>
        void Transaction(Action<IRepository<T>> action);
    }
}