using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.CMS.Testimonials.Dtos;
using DND.Core;
using DND.Domain.CMS.Testimonials;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DND.ApplicationServices.CMS.Testimonials.Services
{
    [ResourceCollection(ResourceCollections.CMS.Testimonials.CollectionId)]
    public class TestimonialApplicationService : ApplicationServiceEntityBase<Testimonial, TestimonialDto, TestimonialDto, TestimonialDto, TestimonialDeleteDto, IAppUnitOfWork>, ITestimonialApplicationService
    {
        public TestimonialApplicationService(IAppUnitOfWork appUnitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<TestimonialDto>> hubContext)
        : base(appUnitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

    }
}
