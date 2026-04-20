using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using SmartAttendance.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace SmartAttendance.Filters
{
    public class ActivityLogFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ActivityLogFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next();

            // After action executed
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userIdClaim = context.HttpContext.User.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    var controllerName = context.RouteData.Values["controller"]?.ToString();
                    var actionName = context.RouteData.Values["action"]?.ToString();
                    var actionString = $"Executed {controllerName}.{actionName}";
                    
                    // Ensure it doesn't exceed 50 chars just in case
                    if (actionString.Length > 50) 
                        actionString = actionString.Substring(0, 50);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var logService = scope.ServiceProvider.GetRequiredService<IActivityLogService>();
                        await logService.LogActionAsync(userId, actionString);
                    }
                }
            }
        }
    }
}
