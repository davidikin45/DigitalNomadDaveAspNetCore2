using AspNetCore.Base.Dtos;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ViewComponents;
using DND.ApplicationServices.CMS.Faqs.Dtos;
using DND.ApplicationServices.CMS.Faqs.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Faq.ViewComponents
{
    public class FaqViewComponent : ViewComponentBase
    {
        private readonly IFaqApplicationService Service;

        public FaqViewComponent(IFaqApplicationService service)
        {
            Service = service;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<FaqDto> data = null;
            int total = 0;

            var dataTask = Service.GetAllAsync(cts.Token, AutoMapperHelper.GetOrderBy<FaqDto>(nameof(FaqDto.CreatedOn), "asc"), null, null, false, false, null);
            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            data = dataTask.Result;
            total = totalTask.Result;


            var response = new WebApiPagedResponseDto<FaqDto>
            {
                Page = 1,
                PageSize = total,
                Records = total,
                Rows = data.ToList()
            };

            ViewBag.Page = 1;
            ViewBag.PageSize = total;

            return View(response);
        }
    }
}
