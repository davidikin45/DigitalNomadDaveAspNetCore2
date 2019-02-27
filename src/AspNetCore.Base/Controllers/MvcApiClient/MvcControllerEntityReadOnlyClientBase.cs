using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Controllers.ApiClient;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System;

namespace AspNetCore.Base.Controllers.MvcApiClient
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [AllowAnonymous]
    public abstract class MvcControllerEntityReadOnlyClientBase<TDto, IEntityService> : MvcControllerEntityReadOnlyClientAuthorizeBase<TDto, IEntityService>
        where TDto : class
        where IEntityService : IApiControllerEntityReadOnlyClient<TDto>
    {

        public MvcControllerEntityReadOnlyClientBase(Boolean admin, IEntityService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
        : base(admin, service, mapper, emailService, appSettings)
        {

        }

    }
}

