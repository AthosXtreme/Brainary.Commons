using Brainary.Commons.Domain;
using Microsoft.EntityFrameworkCore;

namespace Brainary.Commons.Data.Patterns
{
    /// <summary>
    /// Entity Framework <see cref="ISpecification{T}"/> evaluator
    /// </summary>
    internal static class SpecificationEvaluator<T> where T : Entity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var queryable = inputQuery;

            if (specification.Criteria != null)
                queryable = queryable.Where(specification.Criteria);

            queryable = specification.Includes.Aggregate(queryable, (current, include) => EntityFrameworkQueryableExtensions.Include(current, include));
            queryable = specification.IncludeStrings.Aggregate(queryable, (current, include) => EntityFrameworkQueryableExtensions.Include(current, include));

            if (specification.OrderBy != null)
                queryable = queryable.OrderBy(specification.OrderBy);
            else if (specification.OrderByDescending != null)
                queryable = queryable.OrderByDescending(specification.OrderByDescending);

            if (specification.GroupBy != null)
                queryable = queryable.GroupBy(specification.GroupBy).SelectMany(s => s);

            if (specification.IsPagingEnabled)
                queryable = queryable.Skip(specification.Skip).Take(specification.Take);

            if ((specification.Includes.Count + specification.IncludeStrings.Count) > 1)
                queryable = queryable.AsSplitQuery();
            else
                queryable = queryable.AsSingleQuery();

            return queryable;
        }
    }
}
