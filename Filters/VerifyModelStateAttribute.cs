using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System;

namespace VCAPI.Filters
{
    public class VerifyModelStateAttribute : Attribute, IActionFilter
    {
        ///This function is not used, but it has to be there.
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do nothing
        }
        ///Checks if there is missing parameters otherwise returns error 400.
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if(!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState.Values.SelectMany(s => s.Errors).Select(v => v.ErrorMessage));
            }
        }
    }
}
