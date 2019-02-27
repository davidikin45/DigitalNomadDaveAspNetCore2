using AspNetCore.Base.Helpers;
using AspNetCore.Base.ViewComponents;
using DND.ApplicationServices.CMS.ContentHtmls.Dtos;
using DND.ApplicationServices.CMS.ContentHtmls.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.ContentHtml.ViewComponents
{
    [ViewComponent]
    public class ContentHTMLViewComponent : ViewComponentBase
    {
        private readonly IContentHtmlApplicationService Service;

        public ContentHTMLViewComponent(IContentHtmlApplicationService service)
        {
            Service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            ContentHtmlDto data = await Service.GetByIdAsync(id, cts.Token);

            return View(data);
        }
    }
}
