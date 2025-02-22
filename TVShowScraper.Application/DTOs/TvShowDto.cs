namespace TVShowScraper.Application.DTOs;

public class TVShowDto
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<CastDto> Cast { get; set; } = [];
}
