using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RWAProject.Models;

namespace RWAProject.Middleware
{
    public class PermissionsFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;
            if (user.Identity!.Name!.ToLower() != "admin")
            {
                context.Result = new RedirectToActionResult("Index", "AccessDenied", null);
            }
        }
    }
}
