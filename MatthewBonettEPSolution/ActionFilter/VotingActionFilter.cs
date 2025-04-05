using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Presentation.ActionFilter
{
    public class VotingActionFilter : IAsyncActionFilter
    {
        private readonly IPollRepository _pollRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public VotingActionFilter(IPollRepository pollRepository, UserManager<IdentityUser> userManager)
        {
            _pollRepository = pollRepository;
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userId = _userManager.GetUserId(context.HttpContext.User);

            if (userId == null)
            {
                context.Result = new RedirectResult("/Identity/Account/Login");
                return;
            }


            await next();
        }
    }
}
