using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;

namespace VCAPI.Filters
{
    public class AuthorizeProjectRole : ActionFilterAttribute, IAuthorizationFilter 
    {
        IUserRepository users;
        public AuthorizeProjectRole(IUserRepository users)
        {
            this.users = users;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsIdentity claims = context.HttpContext.User.Identities as ClaimsIdentity;
            if(claims != null && claims.IsAuthenticated)
            {
                Claim userNameClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                int projectId;
                try{
                    projectId = int.Parse(context.ActionDescriptor.RouteValues["projectId"]);
                }
                catch(FormatException){
                    context.Result = new BadRequestObjectResult("projectID must be a number");
                    return;
                }
                string username = userNameClaim.Value;

                RANK rank = await users.GetRankForProject(username, projectId);
                claims.AddClaim(new Claim(ClaimTypes.Role, rank.ToString()));
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
