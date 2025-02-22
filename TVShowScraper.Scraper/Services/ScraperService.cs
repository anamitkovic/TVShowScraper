using System.Text.Json;
using TVShowScraper.Application.Interfaces;
using TVShowScraper.Domain.Entities;

namespace TVShowScraper.Scraper.Services;

public class ScraperService(HttpClient httpClient, ITVShowRepository repository, ILogger<ScraperService> logger)
{
	private readonly HttpClient _httpClient = httpClient;
	private readonly ITVShowRepository _repository = repository;
	private readonly ILogger<ScraperService> _logger = logger;
	private const string BaseUrl = "https://api.tvmaze.com";
	private static readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task ScrapeShowsAsync()
	{
		_logger.LogInformation("Starting TV show scraping...");
		var existingShowIds = new HashSet<int>(await _repository.GetAllExternalShowIdsAsync());
		List<TVShow> newShows = [];

		for (int page = 0; ; page++)
		{
			var response = await _httpClient.GetAsync($"{BaseUrl}/shows?page={page}");
			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("API request failed for page {Page}", page);
				break;
			}

			var content = await response.Content.ReadAsStringAsync();
			var shows = JsonSerializer.Deserialize<List<TVShow>>(content, _jsonOptions);

			if (shows == null || shows.Count == 0) break;

			foreach (var show in shows)
			{
				if (!existingShowIds.Contains(show.Id))
				{
					newShows.Add(new TVShow
					{
						ExternalId = show.Id,
						Name = show.Name
					});

					existingShowIds.Add(show.Id);
				}
			}

			while (newShows.Count >= 50)
			{
				var batch = newShows.Take(50).ToList();
				newShows.RemoveRange(0, 50);

				await _repository.AddTvShowsBatchAsync(batch);
			}

			await Task.Delay(500);
		}

		if (newShows.Count > 0)
		{
			await _repository.AddTvShowsBatchAsync(newShows);
		}

		_logger.LogInformation("TV show scraping completed!");
	}

	public async Task ScrapeCastAsync()
	{
		_logger.LogInformation("Starting cast scraping...");

		var showIdMap = (await _repository.GetAllShowIdMappingsAsync())
			.ToDictionary(s => s.ExternalId, s => s.Id);

		_logger.LogInformation("Loaded {Count} show ID mappings.", showIdMap.Count);

		foreach (var externalShowId in showIdMap.Keys)
		{
			await ScrapeCastForShowAsync(externalShowId, showIdMap);
			await Task.Delay(300);
		}

		_logger.LogInformation("Cast scraping completed!");
	}


	private async Task ScrapeCastForShowAsync(int externalShowId, Dictionary<int, int> showIdMap)
	{
		_logger.LogInformation("Fetching cast for ExternalId: {ShowId}", externalShowId);

		if (!showIdMap.TryGetValue(externalShowId, out int showId))
		{
			_logger.LogWarning("Show with ExternalId {ShowId} not found in cache, skipping cast.", externalShowId);
			return;
		}

		var response = await _httpClient.GetAsync($"{BaseUrl}/shows/{externalShowId}/cast");
		if (!response.IsSuccessStatusCode)
		{
			_logger.LogWarning("Failed to fetch cast for show ExternalId {ShowId}", externalShowId);
			return;
		}

		var content = await response.Content.ReadAsStringAsync();
		var castMembers = JsonSerializer.Deserialize<List<CastWrapper>>(content, _jsonOptions);

		if (castMembers == null || castMembers.Count == 0) return;

		var existingCastIds = await _repository.GetExistingCastExternalIdsAsync(showId);

		var newCasts = castMembers
			.Where(c => c.Person != null)
			.Select(c => new Cast
			 {
				ExternalId = c.Person.Id,
				ShowId = showId,
				Name = c.Person.Name,
				Birthday = c.Person.Birthday
				})
			.ToList();


		var uniqueCasts = newCasts
			.GroupBy(c => new { c.ShowId, c.ExternalId })
		    .Select(g => g.First())
			 .ToList();

		var finalCasts = uniqueCasts
			.Where(c => !existingCastIds.Contains(c.ExternalId))
			.ToList();

		if (finalCasts.Count > 0)
		{
			await _repository.AddCastAsync(finalCasts);
			_logger.LogInformation("Added {Count} new cast members for show {ShowId}", uniqueCasts.Count, showId);
		}
		else
		{
			_logger.LogInformation("No new cast members to add for show {ShowId}.", showId);
		}
	}
}

public class CastWrapper
{
	public CastPerson Person { get; set; } = null!;
}

public class CastPerson
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateTime? Birthday { get; set; }
}
