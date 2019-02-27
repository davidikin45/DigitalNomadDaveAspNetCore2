using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.DomainEvents;
using AspNetCore.Base.Email;
using AspNetCore.Base.Reflection;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Base.Controllers.Api
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.

    //[Authorize(Roles = "admin")]
    [AllowAnonymous] // 40
    public abstract class ApiControllerEntityReadOnlyBase<TDto, IEntityService> : ApiControllerEntityReadOnlyAuthorizeBase<TDto, IEntityService>
        where TDto : class
        where IEntityService : IApplicationServiceEntityReadOnly<TDto>
    {   

        public ApiControllerEntityReadOnlyBase(IEntityService service, IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, ITypeHelperService typeHelperService, AppSettings appSettings)
        : base(service, mapper, emailService, linkGenerator, typeHelperService, appSettings)
        {
 
        }

    }
}

