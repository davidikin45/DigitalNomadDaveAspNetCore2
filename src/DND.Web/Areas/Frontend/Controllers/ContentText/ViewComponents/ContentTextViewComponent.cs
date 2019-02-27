using AspNetCore.Base.Helpers;
using AspNetCore.Base.ViewComponents;
using DND.ApplicationServices.CMS.ContentTexts.Dtos;
using DND.ApplicationServices.CMS.ContentTexts.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.ContentText.ViewComponents
{
    public class ContentTextViewComponent : ViewComponentBase
    {
        private readonly IContentTextApplicationService Service;

        public ContentTextViewComponent(IContentTextApplicationService service)
        {
            Service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            ContentTextDto data = await Service.GetByIdAsync(id, cts.Token);

            return View(data);
        }
    }
}
