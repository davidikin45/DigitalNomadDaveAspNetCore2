using AspNetCore.Base.Controllers.Api;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;

namespace DND.Web.ApiControllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class RootController : ApiControllerBase
    {
        public RootController(IMapper mapper, IEmailService emailService, LinkGenerator linkGenerator, AppSettings appSettings)
            : base(mapper, emailService, linkGenerator, appSettings)
        {

        }

        [Route("GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
              new LinkDto(Url.Action(nameof(GetRoot),
              Url.ActionContext.RouteData.Values["controller"].ToString(),
              new { },
              Url.ActionContext.HttpContext.Request.Scheme),
              "self",
              "GET"));

            var apis = new Dictionary<string, string>()
            {
                {"Authors","authors"},
                {"BlogPosts","blog_posts"},
                {"CarouselItems","carousel_items"},
                {"Categories","categories"},
                {"ContentHtmls","content_htmls"},
                {"ContentTexts","content_texts"},
                {"Faqs","faqs"},
                {"Locations","locations"},
                {"MailingList","mailing_list"},
                {"Projects","projects"},
                {"Tags","tags"},
                {"Testimonials","testimonials"},
            };

            foreach (KeyValuePair<string, string> api in apis)
            {
                links.Add(
               new LinkDto(Url.Action("GetPaged",
               api.Key,
               new { },
               Url.ActionContext.HttpContext.Request.Scheme),
              api.Value,
               "GET"));
            }

            return Ok(links);
        }
    }
}
