using System.Linq.Expressions;

namespace Project.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IQueryable to support dynamic sorting and filtering.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Applies dynamic sorting to a queryable.
    /// </summary>
    /// <param name="query">The queryable to sort</param>
    /// <param name="sorting">Sorting string (e.g., "Name ASC", "CreatedOn DESC")</param>
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
            return query;

        try
        {
            // Parse sorting string (e.g., "Name ASC", "CreatedOn DESC")
            var parts = sorting.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return query;

            var propertyName = parts[0];
            var isDescending = parts.Length > 1 && parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);

            // Build expression tree for sorting
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            // Call OrderBy or OrderByDescending
            var methodName = isDescending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(resultExpression);
        }
        catch
        {
            // If sorting fails, return original query
            return query;
        }
    }

    /// <summary>
    /// Applies pagination to a queryable.
    /// </summary>
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
    {
        return query.Skip(skipCount).Take(maxResultCount);
    }

    /// <summary>
    /// Conditionally applies a Where clause if the condition is true.
    /// This is useful for building dynamic queries.
    /// </summary>
    /// <param name="query">The queryable to filter</param>
    /// <param name="condition">If true, the predicate will be applied</param>
    /// <param name="predicate">The filter predicate</param>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }
}
