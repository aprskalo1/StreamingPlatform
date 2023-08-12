using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace RWAProject.Middleware
{
    public class AuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;    
            if (!user.Identity!.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}
