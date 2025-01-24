using AutoMapper;
using Gridify;
using Gridify.EntityFramework;

namespace Dobrasync.Api.BusinessLogic.Extensions;

public static class PaginationExtensions
{
    public static async Task<Paging<D>> PaginateAsync<T, D>(this IQueryable<T> query, GridifyQuery searchQuery,
        IMapper mapper)
    {
        Paging<T> filtered = await query.GridifyAsync(searchQuery);
        List<D> result = mapper.Map<List<D>>(filtered.Data);

        return new Paging<D>() { Count = filtered.Count, Data = result };
    }
}