using AspNetCore.Base.Authorization;
using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices;
using DND.ApplicationServices.Blog.Categories.Dtos;
using DND.ApplicationServices.Blog.Categories.Services;
using Microsoft.AspNetCore.Mvc;

namespace DND.Web.Areas.Admin.Controllers.Categories
{
    [Area("Admin")]
    [ResourceCollection(ResourceCollections.Blog.Categories.CollectionId)]
    [Route("admin/blog/categories")]
    public class CategoriesController : MvcControllerEntityAuthorizeBase<CategoryDto, CategoryDto, CategoryDto, CategoryDeleteDto, ICategoryApplicationService>
    {
        public CategoriesController(ICategoryApplicationService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
             : base(true, service, mapper, emailService, appSettings)
        {
        }
    }
}
