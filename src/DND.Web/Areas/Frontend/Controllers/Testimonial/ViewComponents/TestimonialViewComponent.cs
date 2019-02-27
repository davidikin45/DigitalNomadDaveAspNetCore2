using AspNetCore.Base.Data.RepositoryFileSystem;
using AspNetCore.Base.Helpers;
using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AspNetCore.Base.ViewComponents;
using DND.ApplicationServices.CMS.Testimonials.Dtos;
using DND.ApplicationServices.CMS.Testimonials.Services;
using DND.Web.Areas.Frontend.Controllers.Testimonial.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Areas.Frontend.Controllers.Testimonial.ViewComponents
{
    public class TestimonialViewComponent : ViewComponentBase
    {
        private readonly ITestimonialApplicationService _testimonialService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemRepository;

        public TestimonialViewComponent(ITestimonialApplicationService testimonialService, IFileSystemGenericRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _testimonialService = testimonialService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(TestimonialDto.CreatedOn);
            string orderType = OrderByType.Descending;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<TestimonialDto> testimonials = null;

            var testimonialsTask = _testimonialService.GetAllAsync(cts.Token, AutoMapperHelper.GetOrderBy<TestimonialDto>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, testimonialsTask);

            testimonials = testimonialsTask.Result;


            var viewModel = new TestimonialsViewModel
            {
                Testimonials = testimonials.ToList()
            };

            return View(viewModel);
        }

    }
}
