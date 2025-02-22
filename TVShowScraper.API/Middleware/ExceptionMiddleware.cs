using System.Net;
using System.Text.Json;

namespace TVShowScraper.API.Middleware;
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
	private readonly RequestDelegate _next = next;
	private readonly ILogger<ExceptionMiddleware> _logger = logger;

	public async Task InvokeAsync(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception occurred.");
			await HandleExceptionAsync(context, ex);
		}
	}
	private static Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";
		var response = context.Response;

		object errorResponse;

		switch (exception)
		{
			case ArgumentException:
				response.StatusCode = (int)HttpStatusCode.BadRequest;
				errorResponse = new { message = exception.Message };
				break;

			default:
				response.StatusCode = (int)HttpStatusCode.InternalServerError;
				errorResponse = new { message = "An unexpected error occurred." };
				break;
		}

		return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
	}

}

