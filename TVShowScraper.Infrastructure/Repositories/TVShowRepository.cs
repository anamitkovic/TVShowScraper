using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TVShowScraper.Application.DTOs;
using TVShowScraper.Application.Interfaces;
using TVShowScraper.Domain.Entities;
using TVShowScraper.Infrastructure.DatabaseContext;

namespace TVShowScraper.Infrastructure.Repositories;

public class TVShowRepository(TVShowDbContext dbContext, ILogger<ITVShowRepository> logger) : ITVShowRepository
{
	private readonly TVShowDbContext _dbContext = dbContext;
	private readonly ILogger<ITVShowRepository> _logger = logger;

	public async Task<List<int>> GetAllShowIdsAsync()
	{
		return await _dbContext.TVShows.Select(s => s.Id).ToListAsync();
	}

	public async Task AddShowsAsync(List<TVShow> shows)
	{
		await _dbContext.TVShows.AddRangeAsync(shows);
		await _dbContext.SaveChangesAsync();
	}

	public async Task AddCastAsync(List<Cast> castMembers)
	{
		await _dbContext.Casts.AddRangeAsync(castMembers);
		await _dbContext.SaveChangesAsync();
	}

	public async Task<List<int>> GetAllExternalShowIdsAsync()
	{
		return await _dbContext.TVShows.Select(s => s.ExternalId).ToListAsync();
	}

	public async Task AddTvShowsBatchAsync(List<TVShow> shows)
	{
		if (shows == null || shows.Count == 0) return;

		try
		{
			await _dbContext.TVShows.AddRangeAsync(shows);
			await _dbContext.SaveChangesAsync();
			_logger.LogInformation("Successfully saved batch of {Count} shows.", shows.Count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while saving batch of {Count} shows.", shows.Count);
		}
	}

	public async Task<List<ShowIdMappingDto>> GetAllShowIdMappingsAsync()
	{
		return await _dbContext.TVShows
			.Select(s => new ShowIdMappingDto
			{
				ExternalId = s.ExternalId,
				Id = s.Id
			})
			.ToListAsync();
	}

	public async Task<HashSet<int>> GetExistingCastExternalIdsAsync(int showId)
	{
		return await _dbContext.Casts
			.Where(c => c.ShowId == showId)
			.Select(c => c.ExternalId)
			.ToHashSetAsync();
	}

	public IQueryable<TVShowDto> GetTvShowsWithCast()
	{
		return _dbContext.TVShows
			.Include(t => t.CastMembers)
			.Select(t => new TVShowDto
			{
				Id = t.Id,
				Name = t.Name,
				Cast = t.CastMembers
					.OrderByDescending(c => c.Birthday)
					.Select(c => new CastDto
					{
						Id = c.Id,
						Name = c.Name,
						Birthday = c.Birthday
					})
					.ToList()
			});
	}

}
