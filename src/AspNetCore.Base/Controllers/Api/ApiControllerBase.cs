using AspnetCore.Base.Validation.Errors;
using AspNetCore.Base.ActionResults;
using AspNetCore.Base.Alerts;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.MultiTenancy;
using AspNetCore.Base.Settings;
using AspNetCore.Base.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;

namespace AspNetCore.Base.Controllers.Api
{
    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Consumes("application/json", "application/xml")]
    [Produces("application/json", "application/xml")]
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public LinkGenerator LinkGenerator { get; }
        public AppSettings AppSettings { get; }

        public ApiControllerBase()
        {

        }

        public ApiControllerBase(IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, AppSettings appSettings)
        {
            Mapper = mapper;
            EmailService = emailService;
            LinkGenerator = linkGenerator;
            AppSettings = appSettings;
        }

        //https://docs.microsoft.com/en-us/aspnet/core/migration/claimsprincipal-current?view=aspnetcore-2.0
        public string Username
        {
            get
            {
                if (User != null && User.Identity != null && !string.IsNullOrEmpty(User.Identity.Name))
                {
                    return User.Identity.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        public string UserId
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return null;
                }

                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    return null;
                }

                return claim.Value;
            }
        }

        public AppTenant Tenant
        {
            get
            {
                object multiTenant;
                if (!HttpContext.Items.TryGetValue("_tenant", out multiTenant))
                    throw new ApplicationException("Could not find tenant.");

                return (AppTenant)multiTenant;
            }
        }

        protected IActionResult BulkTriggerActionResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.ActionSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return Success(webApiMessages);
        }

        protected List<WebApiMessage> BulkCreateResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.AddSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return webApiMessages;
        }

        protected List<WebApiMessage> BulkUpdateResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if(result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.UpdateSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return webApiMessages;
        }

        protected List<WebApiMessage> BulkDeleteResponse(IEnumerable<Result> results)
        {
            var webApiMessages = new List<WebApiMessage>();

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    webApiMessages.Add(WebApiMessage.CreateWebApiMessage(Messages.DeleteSuccessful, new List<string>()));
                }
                else
                {
                    webApiMessages.Add((WebApiMessage)((ObjectResult)ValidationErrors(result)).Value);
                }
            }

            //For bulk return 200 regardless
            return webApiMessages;
        }

        protected ActionResult ValidationErrors(Result failure)
        {
            var newModelState = new ModelStateDictionary();
            switch (failure.ErrorType)
            {
                case ErrorType.ObjectValidationFailed:
                    newModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    break;
                case ErrorType.ObjectDoesNotExist:
                    return ApiNotFoundErrorMessage(Messages.NotFound);
                case ErrorType.ConcurrencyConflict:
                    newModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    break;
                default:
                    //perhaps should be throwing so Startup returns a 500
                    //throw ex;
                    newModelState.AddModelError("", Messages.UnknownError);
                    break;
            }
            return ValidationErrors(newModelState);
        }

        protected ActionResult ValidationErrors()
        {
            return ValidationErrors(Messages.RequestInvalid, ModelState);
        }

        protected ActionResult ValidationErrors(ModelStateDictionary modelState)
        {
            return ValidationErrors(Messages.RequestInvalid, modelState);
        }

        protected virtual ActionResult ValidationErrors(string message, ModelStateDictionary modelState)
        {
            return new UnprocessableEntityAngularObjectResult(message, modelState);
        }

        protected virtual ActionResult Success<T>(T model)
        {
            return new OkObjectResult(model);
        }

        protected CancellationToken ClientDisconnectedToken()
        {
            return this.HttpContext.RequestAborted;
        }

        protected ActionResult ApiBadRequest()
        {
            return ApiErrorMessage(Messages.RequestInvalid);
        }

        protected ActionResult ApiErrorMessage(string message)
        {
            return ApiErrorMessage(Messages.RequestInvalid, message);
        }

        protected ActionResult ApiNotFound()
        {
            return ApiNotFoundErrorMessage(Messages.NotFound);
        }

        protected ActionResult ApiNotFoundErrorMessage(string message)
        {
            return ApiErrorMessage(Messages.NotFound, message, 404);
        }

        protected virtual ActionResult ApiErrorMessage(string message, string errorMessage, int errorStatusCode = 400)
        {
            var errorList = new List<string>();
            errorList.Add(errorMessage);

            var response = WebApiMessage.CreateWebApiMessage(message, errorList);

            var result = new ObjectResult(response);
            result.StatusCode = errorStatusCode;

            return result;
        }

        protected virtual IActionResult ApiCreatedSuccessMessage(string message, Object id)
        {
            return ApiSuccessMessage(message, id, HttpStatusCode.Created);
        }

        protected virtual IActionResult ApiSuccessMessage(string message, Object id, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var errorList = new List<string>();

            var response = WebApiMessage.CreateWebApiMessage(message, errorList, id);

            var result = new ObjectResult(response);
            result.StatusCode = (int)statusCode;

            return result;
        }

        protected virtual IActionResult Html(string html)
        {
            return new HTMLActionResult(html);
        }

        protected virtual IActionResult Forbidden()
        {
            return ApiErrorMessage(Messages.Forbidden, Messages.Forbidden, 403);
        }

    }
}

