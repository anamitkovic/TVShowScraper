using Microsoft.EntityFrameworkCore;
using TVShowScraper.API.Middleware;
using TVShowScraper.Application.Interfaces;
using TVShowScraper.Application.Services;
using TVShowScraper.Infrastructure.DatabaseContext;
using TVShowScraper.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TVShowDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITVShowRepository, TVShowRepository>();
builder.Services.AddScoped<ITVShowService, TvShowService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "TV Show API v1");
		c.RoutePrefix = "swagger"; 
	});
}
app.UseMiddleware<ExceptionMiddleware>(); 
app.UseAuthorization();
app.MapControllers();
app.Run();
