using AspNetCore.Base.Alerts;
using AspNetCore.Base.Validation;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Base.Controllers.Api
{
    //using CSharpFunctionalExtensions;
    public abstract class ApiControllerCQRSBase : ControllerBase
    {
        protected new IActionResult Ok()
        {
            return base.Ok(Envelope.Ok());
        }

        protected IActionResult Ok<T>(T result)
        {
            return base.Ok(Envelope.Ok(result));
        }

        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(Envelope.Error(errorMessage));
        }

        protected IActionResult FromResult(Result result)
        {
            return result.IsSuccess ? Ok() : Error("");
        }
    }
}
