using TVShowScraper.Application.DTOs;
using TVShowScraper.Application.Extensions;
using TVShowScraper.Application.Interfaces;

namespace TVShowScraper.Application.Services;
public class TvShowService(ITVShowRepository tvShowRepository) : ITVShowService
{
	private readonly ITVShowRepository _tvShowRepository = tvShowRepository;

	public async Task<PagedResult<TVShowDto>> GetTvShowsAsync(int page, int pageSize)
	{
		return await _tvShowRepository.GetTvShowsWithCast().PaginateAsync(page, pageSize);
	}
}


