using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace tests.TestControllers
{
    public abstract class ControllerTestUtility
    {
         public static void SetCallersUsername(string name, Controller controller)
        {
            controller.ControllerContext = new ControllerContext{
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]{
                                new Claim(ClaimTypes.NameIdentifier, name)
                            }))
                }
            };
        }
    }
}