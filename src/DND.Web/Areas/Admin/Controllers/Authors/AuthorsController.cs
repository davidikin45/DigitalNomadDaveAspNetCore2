using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Authors.Dtos;
using DND.ApplicationServices.Blog.Authors.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Authors
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.Blog.Authors.CollectionId)]
    [Route("admin/blog/authors")]
    public class AuthorsController : MvcControllerEntityAuthorizeBase<AuthorDto, AuthorDto, AuthorDto, AuthorDeleteDto, IAuthorApplicationService>
    {
        public AuthorsController(IAuthorApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
