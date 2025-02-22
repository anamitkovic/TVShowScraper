using TVShowScraper.Application.DTOs;

namespace TVShowScraper.Application.Interfaces
{
    public interface ITVShowService
    {
		Task<PagedResult<TVShowDto>> GetTvShowsAsync(int page, int pageSize);

	}
}
