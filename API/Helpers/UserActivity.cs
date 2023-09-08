using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class UserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
             var resultContext = await next();

             if(!resultContext.HttpContext.User.Identity!.IsAuthenticated) return;

             var userId = resultContext.HttpContext.User.GetUserId();
             var unit = resultContext.HttpContext.RequestServices.GetService<IUnitOfWorks>();

             var user = await unit!.UserRepository!.GetUsersByIdAsync(userId);
              user!.LastActive = DateTime.UtcNow;

              await unit.Complete();
        }
    }
}