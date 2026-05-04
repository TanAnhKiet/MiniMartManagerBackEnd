using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiniMartManagementAPI.Filtter
{
    public class AuditLogFilter : IAsyncActionFilter
    {
        private readonly ILogger<AuditLogFilter> _logger;

        public AuditLogFilter(ILogger<AuditLogFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            
            // Chỉ log những thao tác thay đổi dữ liệu
            if (method == "POST" || method == "PUT" || method == "DELETE")
            {
                var user = context.HttpContext.User.FindFirstValue(ClaimTypes.Name) 
                           ?? context.HttpContext.User.FindFirstValue(BackEnd.Core.SeedWorks.Constans.UserClaims.UserId) 
                           ?? "Anonymous";
                           
                var path = context.HttpContext.Request.Path;
                
                _logger.LogInformation("User {User} initiated {Method} request to {Path}", user, method, path);
                
                var resultContext = await next(); // Execute the action

                if (resultContext.Exception != null)
                {
                    _logger.LogError(resultContext.Exception, "Error during {Method} request to {Path} by {User}", method, path, user);
                }
                else
                {
                    _logger.LogInformation("{Method} request to {Path} completed by {User}", method, path, user);
                }
            }
            else
            {
                await next();
            }
        }
    }
}
