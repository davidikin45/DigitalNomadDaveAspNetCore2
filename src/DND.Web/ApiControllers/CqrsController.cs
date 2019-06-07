using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Cqrs;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace DND.Web.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cqrs")]
    public class CqrsController : ApiControllerCqrsBase
    {
        public CqrsController(ICqrsMediator cqrsMediator, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, AppSettings appSettings)
            : base(cqrsMediator, mapper, emailService, linkGenerator, appSettings)
        {

        }
    }
}
