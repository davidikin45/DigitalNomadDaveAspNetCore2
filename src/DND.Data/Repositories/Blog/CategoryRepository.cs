using AspNetCore.Base.Data.Repository;
using AspNetCore.Base.Helpers;
using DND.Core.Repositories.Blog;
using DND.Domain.Blog.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Data.Repositories.Blog
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppContext context)
            :base(context)
        {

        }

        public async Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            return await GetFirstAsync(cancellationToken, c => c.UrlSlug.Equals(categorySlug)).ConfigureAwait(false);
        }

        public override async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, int? skip = null, int? take = null, bool getAggregate = false, bool getAggregateAndAssociatedAggregates = false, params Expression<Func<Category, object>>[] includeProperties)
        {
            return await GetAllAsync(cancellationToken, o => o.OrderBy(c => c.Name)).ConfigureAwait(false);
        }

        public override Category Add(Category entity, string addedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.Add(entity, addedBy);
        }

        public override Category Update(Category entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.Update(entity, updatedBy);
        }
    }
}
