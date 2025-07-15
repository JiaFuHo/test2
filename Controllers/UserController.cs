using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace test2.Controllers
{
    public abstract class UserController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier);
            var userName = User.FindFirst("Name");

            ViewData["UserId"] = (userId != null) ? userId.Value : string.Empty;
            ViewData["UserName"] = (userName != null) ? userName.Value : string.Empty;

            base.OnActionExecuting(context);
        }
    }
}