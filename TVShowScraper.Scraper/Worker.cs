using Microsoft.Extensions.DependencyInjection;
using TVShowScraper.Scraper.Services;

namespace TVShowScraper.Scraper;

public class Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
	private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Starting TV Show Scraper...");

		using (var scope = _serviceScopeFactory.CreateScope())
		{
			var scraperService = scope.ServiceProvider.GetRequiredService<ScraperService>();

			await scraperService.ScrapeShowsAsync();
			await scraperService.ScrapeCastAsync();
		}


		_logger.LogInformation("Scraping completed!");
	}
}
