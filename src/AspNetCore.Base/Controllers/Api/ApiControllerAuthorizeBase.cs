using AspNetCore.Base.Authentication;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AspNetCore.Base.Controllers.Api
{


    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    //[Authorize(Roles = "admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme+ "," + BasicAuthenticationDefaults.AuthenticationScheme, Roles = "admin")] // 40
    public abstract class ApiControllerAuthorizeBase : ApiControllerBase
    {
        public ApiControllerAuthorizeBase(IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, AppSettings appSettings)
            :base(mapper, emailService, linkGenerator, appSettings)
        {
           
        }
    }
}

