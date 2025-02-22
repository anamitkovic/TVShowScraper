using Microsoft.AspNetCore.Mvc;
using Moq;
using TVShowScraper.API;
using TVShowScraper.Application.DTOs;
using TVShowScraper.Application.Interfaces;

namespace TVShowScraper.Tests;
public class TVControllersTests
{
	[Fact]
	public async Task GetTvShows_ReturnsOkResult()
	{
		var mockService = new Mock<ITVShowService>();
		var controller = new TvShowsController(mockService.Object);

		var tvShows = new PagedResult<TVShowDto>(
			[
				new TVShowDto { Id = 1, Name = "Breaking Bad", Cast = new List<CastDto>() }
			],
			1, 1, 10
		);

		mockService.Setup(s => s.GetTvShowsAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(tvShows);

		var result = await controller.GetTvShows(1, 10);

		var okResult = Assert.IsType<OkObjectResult>(result);
		var returnValue = Assert.IsType<PagedResult<TVShowDto>>(okResult.Value);
		Assert.Single(returnValue.Items);
	}

}

