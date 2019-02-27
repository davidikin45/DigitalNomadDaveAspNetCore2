using AspNetCore.Base.Alerts;
using AspNetCore.Base.Email;
using AspNetCore.Base.Extensions;
using AspNetCore.Base.Settings;
using AspNetCore.Base.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AspNetCore.Base.Controllers.Mvc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class MvcControllerBase : Controller
    {
        public IMapper Mapper { get; }
        public IEmailService EmailService { get; }
        public AppSettings AppSettings { get; }

        public MvcControllerBase()
        {

        }

        public MvcControllerBase(IMapper mapper = null, IEmailService emailService = null, AppSettings appSettings = null)
        {
            Mapper = mapper;
            EmailService = emailService;
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
                    return "Anonymous";
                }
            }
        }

        protected ActionResult RedirectToAction<TController>(Expression<Action<TController>> action)
            where TController : Controller
        {
            return ControllerExtensions.RedirectToAction(this, action);
        }

        protected CancellationToken ClientDisconnectedToken()
        {
            return HttpContext.RequestAborted;
        }

        protected ActionResult HandleReadException()
        {
            return RedirectToControllerDefault().WithError(this, Messages.RequestInvalid);
        }

        protected void HandleUpdateException(Result failure, object dto, bool clearPostData)
        {
            //TODO: Need to research how to turn off automatic model validation if doing it in application service layer
            //Clears all Post Data
            if (clearPostData)
            {
                ModelState.Clear();
            }

            switch (failure.ErrorType)
            {
                case ErrorType.ObjectValidationFailed:
                    ModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    break;
                case ErrorType.ConcurrencyConflict:
                    ModelState.AddValidationErrors(failure.ObjectValidationErrors);
                    //if(dto is IDtoConcurrencyAware)
                    //{
                    //    ((IDtoConcurrencyAware)dto).RowVersion = failure.NewRowVersion;
                    //}
                    //Update RowVersion on DTO
                    break;
                default:
                    ModelState.AddModelError("", Messages.UnknownError);
                    break;
            }
        }

        protected void HandleUpdateException(Exception ex)
        {
            ModelState.AddModelError("", Messages.UnknownError);
        }

        protected virtual ActionResult RedirectToHome()
        {
            return RedirectToRoute("Default");
        }

        protected virtual ActionResult RedirectToControllerDefault()
        {
            return RedirectToAction("Index");
        }

        protected string ControllerName
        {
            get { return this.ControllerContext.RouteData.Values["controller"].ToString(); }
        }

        protected string Title
        {
            get { return this.HttpContext.Request.Path; }
        }

        protected Task<string> GetTokenAsync()
        {
           return HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");
        }
    }
}
