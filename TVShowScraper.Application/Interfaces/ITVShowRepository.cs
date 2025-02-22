using TVShowScraper.Application.DTOs;
using TVShowScraper.Domain.Entities;

namespace TVShowScraper.Application.Interfaces;
public interface ITVShowRepository
{
	Task AddShowsAsync(List<TVShow> shows);
	Task AddCastAsync(List<Cast> castMembers);
	Task<List<int>> GetAllExternalShowIdsAsync();
	Task AddTvShowsBatchAsync(List<TVShow> shows);
	Task<List<ShowIdMappingDto>> GetAllShowIdMappingsAsync();
	Task<HashSet<int>> GetExistingCastExternalIdsAsync(int showId);
	IQueryable<TVShowDto> GetTvShowsWithCast();

}

