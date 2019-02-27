using AspNetCore.Base.ApplicationServices;
using AspNetCore.Base.Authorization;
using AspNetCore.Base.SignalR;
using AspNetCore.Base.Users;
using AspNetCore.Base.Validation;
using AutoMapper;
using DND.ApplicationServices.Blog.BlogPosts.Dtos;
using DND.Core;
using DND.Domain.Blog.BlogPosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.BlogPosts.Services
{
    [ResourceCollection(ResourceCollections.Blog.BlogPosts.CollectionId)]
    public class BlogPostApplicationService : ApplicationServiceEntityBase<BlogPost, BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto, IAppUnitOfWork>, IBlogPostApplicationService
    {
        public BlogPostApplicationService(IAppUnitOfWork unitOfWork, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService, IHubContext<ApiNotificationHub<BlogPostDto>> hubContext)
        : base(unitOfWork, mapper, authorizationService, userService, validationService, hubContext)
        {

        }

        public override void AddIncludes(List<Expression<Func<BlogPost, object>>> includes)
        {
            includes.Add(p => p.Tags);
            includes.Add(p => p.Locations);
        }

        public async Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken)
        {
            return await UnitOfWork.BlogPostRepository.GetTotalPostsAsync(checkIsPublished, cancellationToken);
        }

        public IEnumerable<BlogPostDto> GetPosts(int pageNo, int pageSize)
        {
            var posts = UnitOfWork.BlogPostRepository.GetPosts(pageNo, pageSize);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsAsync(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsAsyncWithLocation(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsForCarouselAsync(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsForAuthorAsync(authorSlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            return await UnitOfWork.BlogPostRepository.GetTotalPostsForAuthorAsync(authorSlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsForCategoryAsync(categorySlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            return await UnitOfWork.BlogPostRepository.GetTotalPostsForCategoryAsync(categorySlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsForTagAsync(tagSlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            return await UnitOfWork.BlogPostRepository.GetTotalPostsForTagAsync(tagSlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await UnitOfWork.BlogPostRepository.GetPostsForSearchAsync(search, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            return await UnitOfWork.BlogPostRepository.GetTotalPostsForSearchAsync(search, cancellationToken);
        }

        public async Task<BlogPostDto> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            var bo = await UnitOfWork.BlogPostRepository.GetPostAsync(year, month, titleSlug, cancellationToken);
            return Mapper.Map<BlogPostDto>(bo);
        }
    }
}
