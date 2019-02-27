using AspnetCore.Base.Validation.Errors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace AspNetCore.Base.Middleware
{
    public static class MvcErrorHandler
    {
        public static (IActionResult result, bool exceptionHandled) HandleException(ClaimsPrincipal user, Exception exception)
        {
            bool exceptionHandled = false;
            IActionResult result = null;

            if (exception is UnauthorizedErrors)
            {
                if(user.Identity.IsAuthenticated)
                {
                    result = new ForbidResult();
                }
                else
                {
                    result = new ChallengeResult();
                }
                exceptionHandled = true;
            }
            else if (exception is OperationCanceledException)
            {

            }
            else if (exception is TimeoutException)
            {

            }
            else
            {

            }

            return (result, exceptionHandled);
        }
    }
}
