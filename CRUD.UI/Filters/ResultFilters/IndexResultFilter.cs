using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDUI.Filters.ResultFilters
{
    public class IndexResultFilter : IAsyncResultFilter
    {
        private readonly ILogger<IndexResultFilter> _logger;

        public IndexResultFilter(ILogger<IndexResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("Result filter before IActionResult Execution");

            await next();

            _logger.LogInformation("Result filter after IActionResult Execution");
            context.HttpContext.Response.Headers["Last-Modified"] = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
        }
    }
}
