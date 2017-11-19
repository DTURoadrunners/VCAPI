using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using VCAPI.Repository.Interfaces;

namespace VCAPI.Filters
{
    public class Verifier : ActionFilterAttribute, IAuthorizationFilter 
    {
        IUserRepository users;
        public Verifier(IUserRepository users)
        {
            this.users = users;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            ClaimsIdentity claims = context.HttpContext.User.Identities as ClaimsIdentity;
            if(claims != null && claims.IsAuthenticated)
            {
                Claim userNameClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                int projectId = int.Parse(context.ActionDescriptor.RouteValues["projectId"]);

                string username = userNameClaim.Value;

                RANK rank = await users.GetRankForProject(username, projectId);
                claims.AddClaim(new Claim(ClaimTypes.Role, rank.ToString()));
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
