using AspNetCore.Base.Data.UnitOfWork;
using AspNetCore.Base.DomainEvents;
using AspNetCore.Base.Validation;
using DND.Core;
using DND.Core.Repositories.Blog;
using DND.Data.Repositories.Blog;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Tags;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DND.Data
{
    public class AppUnitOfWork : UnitOfWorkWithEventsBase, IAppUnitOfWork
    {
        public IAuthorRepository AuthorRepository { get; private set; }
        public IBlogPostRepository BlogPostRepository { get; private set; }
        public ICategoryRepository CategoryRepository { get; private set; }
        public ILocationRepository LocationRepository { get; private set; }
        public ITagRepository TagRepository { get; private set; }

        public AppUnitOfWork(IValidationService validationService, IDomainEventsMediator domainEventsMediator, AppContext appContext)
            :base(true, validationService, domainEventsMediator, appContext)
        {

        }

        public override void InitializeRepositories(Dictionary<Type, DbContext> contextsByEntityType)
        {
            AuthorRepository = new AuthorRepository((AppContext)contextsByEntityType[typeof(Author)]);
            BlogPostRepository = new BlogPostRepository((AppContext)contextsByEntityType[typeof(Author)]);
            CategoryRepository = new CategoryRepository((AppContext)contextsByEntityType[typeof(Category)]);
            LocationRepository = new LocationRepository((AppContext)contextsByEntityType[typeof(Location)]);
            TagRepository = new TagRepository((AppContext)contextsByEntityType[typeof(Tag)]);

            AddRepository(AuthorRepository);
            AddRepository(BlogPostRepository);
            AddRepository(CategoryRepository);
            AddRepository(LocationRepository);
            AddRepository(TagRepository);
        }
    }
}
