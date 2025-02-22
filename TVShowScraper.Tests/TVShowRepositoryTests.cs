using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TVShowScraper.Application.Interfaces;
using TVShowScraper.Domain.Entities;
using TVShowScraper.Infrastructure.DatabaseContext;
using TVShowScraper.Infrastructure.Repositories;

namespace TVShowScraper.Tests;

public class TVShowRepositoryTests
{
	private readonly Mock<ILogger<TVShowRepository>> _mockLogger;
	private readonly ITVShowRepository _repository;
	private readonly TVShowDbContext _dbContext;

	public TVShowRepositoryTests()
	{
		var options = new DbContextOptionsBuilder<TVShowDbContext>()
			.UseInMemoryDatabase(databaseName: "TestDatabase")
			.Options;

		_dbContext = new TVShowDbContext(options);
		_mockLogger = new Mock<ILogger<TVShowRepository>>();

		_repository = new TVShowRepository(_dbContext, _mockLogger.Object);
	}

	[Fact]
	public async Task GetTvShowsWithCastAsync_ReturnsTVShows()
	{
		var show = new TVShow { Id = 1, Name = "Breaking Bad" };
		_dbContext.TVShows.Add(show);
		await _dbContext.SaveChangesAsync();

		var result = _repository.GetTvShowsWithCast();

		Assert.Contains(result, x => x.Name == "Breaking Bad");
	}

	[Fact]
	public async Task AddTvShowAsync_SavesSuccessfully()
	{
		var show = new List<TVShow> { new() { Id = 2, Name = "Stranger Things" } };

		await _repository.AddShowsAsync(show);
		var savedShow = await _dbContext.TVShows.FirstOrDefaultAsync(s => s.Id == 2);

		Assert.NotNull(savedShow);
		Assert.Equal("Stranger Things", savedShow.Name);
	}

	[Fact]
	public async Task AddCastAsync_DuplicatesIgnored()
	{

		var castMembers = new List<Cast>
		{
			new() { ExternalId = 101, ShowId = 1, Name = "Bryan Cranston" },
			new() { ExternalId = 101, ShowId = 1, Name = "Bryan Cranston" }
		};

		await _repository.AddCastAsync(castMembers);

		var result = await _repository.GetExistingCastExternalIdsAsync(1);

		Assert.Single(result); 
	}

	[Fact]
	public async Task GetTvShowsWithCastAsync_ReturnsPaginatedShows()
	{
		
		var shows = new List<TVShow>
		{
			new() { Id = 1, Name = "Breaking Bad" },
			new() { Id = 2, Name = "Game of Thrones" }
		};

		var castMembers = new List<Cast>
	    {
			new() { Id = 1, ShowId = 1, Name = "Bryan Cranston", Birthday = new DateTime(1956, 3, 7) },
			new() { Id = 2, ShowId = 2, Name = "Peter Dinklage", Birthday = new DateTime(1969, 6, 11) }
		};

		_dbContext.TVShows.AddRange(shows);
		_dbContext.Casts.AddRange(castMembers);
		await _dbContext.SaveChangesAsync();

		var result = _repository.GetTvShowsWithCast().ToList();

		Assert.Equal(2, result.Count);
		Assert.Equal("Breaking Bad", result.First().Name);
	}


}
