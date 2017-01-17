using System.Linq;

namespace GovITHub.Auth.Common.Data
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Apply<T>(this IQueryable<T> query, Data.ModelQueryFilter queryFilter)
        {
            if (queryFilter != null)
            {
                query = query
                    .Skip(queryFilter.CurrentPage * queryFilter.ItemsPerPage)
                    .Take(queryFilter.ItemsPerPage);

                if (!string.IsNullOrEmpty(queryFilter.SortBy))
                {
                    if (queryFilter.SortAscending)
                    {
                        query = query.OrderBy(queryFilter.SortBy);
                    }
                    else
                    {
                        query = query.OrderByDescending(queryFilter.SortBy);
                    }
                }
            }

            return query;
        }
    }
}