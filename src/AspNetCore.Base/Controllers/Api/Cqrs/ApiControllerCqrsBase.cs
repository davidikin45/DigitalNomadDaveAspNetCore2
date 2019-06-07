using AspNetCore.Base.Cqrs;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Base.Controllers.Api
{
    [AllowAnonymous]
    public abstract class ApiControllerCqrsBase : ApiControllerCqrsAuthorizeBase
    {

        public ApiControllerCqrsBase(ICqrsMediator cqrsMediator, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, AppSettings appSetings)
        : base(cqrsMediator, mapper, emailService, linkGenerator, appSetings)
        {
           
        }
    }
}

