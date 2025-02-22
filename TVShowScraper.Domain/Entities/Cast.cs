using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TVShowScraper.Domain.Entities;
public class Cast
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	public int ShowId { get; set; }
	public string Name { get; set; } = string.Empty;
	public DateTime? Birthday { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public int ExternalId { get; set; }
	public TVShow Show { get; set; } = null!;
}


