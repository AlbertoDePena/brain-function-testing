using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace BFT.Extensions
{
    public static class Extensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IDocumentQuery<T> queryable)
        {
            var list = new List<T>();

            while (queryable.HasMoreResults)
            {
                list.AddRange(await queryable.ExecuteNextAsync<T>());
            }

            return list;
        }
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query) => await query.AsDocumentQuery().ToListAsync();
    }
}