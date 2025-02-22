using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TVShowScraper.Domain.Entities;

namespace TVShowScraper.Infrastructure.DatabaseContext;
public class TVShowDbContext(DbContextOptions<TVShowDbContext> options) : DbContext(options)
{
	public DbSet<TVShow> TVShows { get; set; }
	public DbSet<Cast> Casts { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<TVShow>()
			.HasMany(t => t.CastMembers)
			.WithOne(c => c.Show)
			.HasForeignKey(c => c.ShowId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}


