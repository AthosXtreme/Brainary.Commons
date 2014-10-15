namespace Brainary.Commons.Domain.Contracts
{
    using System;
    using System.Linq.Expressions;
    using Data;
    using Data.Patterns.Specification;

    /// <summary>
    /// Specification query for <see cref="IEntity"/> interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpecificationQuery<T> where T : class, IEntity
    {
        IResultQuery<T> Query(Expression<Func<T, bool>> func, int pageIndex, int resultPerPage);

        IResultQuery<T> Query(ISpecification<T> specification, int pageIndex, int resultPerPage);

        IResultQuery<T> Query<TProperty>(ISpecification<T> specification, int pageIndex, int resultPerPage, Expression<Func<T, TProperty>> orderBy, bool ascending);
    }
}