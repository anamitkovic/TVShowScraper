using Microsoft.EntityFrameworkCore;
using TVShowScraper.Scraper.Services;
using TVShowScraper.Infrastructure.Repositories;
using TVShowScraper.Application.Interfaces;
using TVShowScraper.Infrastructure.DatabaseContext;
using TVShowScraper.Scraper;

var host = Host.CreateDefaultBuilder(args)
	.ConfigureServices((context, services) =>
	{
		services.AddDbContext<TVShowDbContext>(options =>
			options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

		services.AddScoped<ITVShowRepository, TVShowRepository>();
		services.AddHttpClient<ScraperService>();
		services.AddHostedService<Worker>();
	})
	.Build();

await host.RunAsync();
