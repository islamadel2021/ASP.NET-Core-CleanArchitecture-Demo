using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDUI.Filters.ResultFilters
{
    public class TokenResultFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await next();
            context.HttpContext.Response.Cookies.Append("Token-Key", "Muhammad");
        }
    }
}
