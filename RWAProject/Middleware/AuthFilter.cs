using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RWAProject.Middleware
{
    public class AuthFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Session.TryGetValue("userToken", out _))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }
        }
    }
}
