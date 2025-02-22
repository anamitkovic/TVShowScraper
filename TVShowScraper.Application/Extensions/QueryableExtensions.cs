using Microsoft.EntityFrameworkCore;
using TVShowScraper.Application.DTOs;

namespace TVShowScraper.Application.Extensions;
public static class QueryableExtensions
{
	public static async Task<PagedResult<T>> PaginateAsync<T>(this IQueryable<T> query, int page, int pageSize)
	{
		var totalCount = await query.CountAsync();

		var items = await query
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return new PagedResult<T>(items, totalCount, page, pageSize);
	}

}
