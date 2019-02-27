using AspNetCore.Base.Controllers.Mvc;
using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Dtos;
using AspNetCore.Base.Email;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.MvcFeatures;
using AspNetCore.Base.Settings;
using AutoMapper;
using DND.ApplicationServices.Blog.Locations.Dtos;
using DND.ApplicationServices.Blog.Locations.Services;
using DND.Domain.Blog.Locations;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Countries
{
    [Area("Frontend")]
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Countries" })]
    [Route("countries")]
    public class CountriesController : MvcControllerBase
    {
        private readonly ILocationApplicationService _locationService;

        public CountriesController(ILocationApplicationService locationService, IMapper mapper, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IEmailService emailService, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
            _locationService = locationService;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 20, string orderColumn = nameof(LocationDto.Name), string orderType = "asc", string search = "")
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var dataTask = _locationService.SearchAsync(cts.Token, null, LocationType.Country.ToString() + "&" + search, l => !string.IsNullOrEmpty(l.Album) && !string.IsNullOrEmpty(l.UrlSlug), AutoMapperHelper.GetOrderBy<LocationDto>(orderColumn, orderType), page - 1, pageSize);
                var totalTask = _locationService.GetSearchCountAsync(cts.Token, null, LocationType.Country.ToString() + "&" + search, l => !string.IsNullOrEmpty(l.Album) && !string.IsNullOrEmpty(l.UrlSlug));

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDto<LocationDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = data.ToList(),
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search
                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                return View(response);
            }
            catch
            {
                return HandleReadException();
            }
        }
    }
}
