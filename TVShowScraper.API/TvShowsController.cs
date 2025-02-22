using Microsoft.AspNetCore.Mvc;
using TVShowScraper.Application.DTOs;
using TVShowScraper.Application.Interfaces;

namespace TVShowScraper.API;

[ApiController]
[Route("api/tvshows")]
public class TvShowsController(ITVShowService tvShowService) : ControllerBase
{
	private readonly ITVShowService _tvShowService = tvShowService;

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<TVShowDto>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetTvShows([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
	{
		if (page < 1 || pageSize < 1)
		{
			throw new ArgumentException("Page and pageSize must be greater than 0.");
		}

		var result = await _tvShowService.GetTvShowsAsync(page, pageSize);
		return Ok(result);
	}
}
