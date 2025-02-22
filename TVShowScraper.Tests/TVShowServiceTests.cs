using Moq;
using System;
using System.Collections.Generic;
using TVShowScraper.Application.DTOs;
using TVShowScraper.Application.Interfaces;
using TVShowScraper.Application.Services;

namespace TVShowScraper.Tests;

public class TVShowServiceTests
{

	[Fact]
	public async Task GetTvShowsAsync_ReturnsPaginatedList()
	{

		var mockRepo = new Mock<ITVShowRepository>();
		var service = new TvShowService(mockRepo.Object);

		var shows = new List<TVShowDto>
		{
			new() { Id = 1, Name = "Breaking Bad", Cast = [] },
			new() { Id = 2, Name = "Game of Thrones", Cast = [] }
		};

		mockRepo.Setup(r => r.GetTvShowsWithCast()).Returns(shows.AsQueryable);


		var result = await service.GetTvShowsAsync(1, 10);
		Assert.Equal(2, result.Items.Count);
		Assert.Equal("Breaking Bad", result.Items.First().Name);
	}

}
