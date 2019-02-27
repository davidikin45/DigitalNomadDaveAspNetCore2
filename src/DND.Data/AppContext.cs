using AspNetCore.Base.Data;
using DND.Data.Configurations.Blog.Authors;
using DND.Data.Configurations.Blog.BlogPosts;
using DND.Data.Configurations.Blog.Categories;
using DND.Data.Configurations.Blog.Locations;
using DND.Data.Configurations.Blog.Tags;
using DND.Data.Configurations.CMS.CarouselItems;
using DND.Data.Configurations.CMS.ContentHtmls;
using DND.Data.Configurations.CMS.ContentTexts;
using DND.Data.Configurations.CMS.Faqs;
using DND.Data.Configurations.CMS.MailingLists;
using DND.Data.Configurations.CMS.Projects;
using DND.Data.Configurations.CMS.Testimonials;
using DND.Domain.Blog.Authors;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.Categories;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Tags;
using DND.Domain.CMS.CarouselItems;
using DND.Domain.CMS.ContentHtmls;
using DND.Domain.CMS.ContentTexts;
using DND.Domain.CMS.Faqs;
using DND.Domain.CMS.MailingLists;
using DND.Domain.CMS.Projects;
using DND.Domain.CMS.Testimonials;
using Microsoft.EntityFrameworkCore;

namespace DND.Data
{
    public class AppContext : DbContextBase
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<BlogPostLocation> BlogPostLocations { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<CarouselItem> CarouselItems { get; set; }
        public DbSet<ContentHtml> ContentHtml { get; set; }
        public DbSet<ContentText> ContentText { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<MailingList> MailingList { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }

        public AppContext()
        {

        }

        public AppContext(DbContextOptions<AppContext> options = null) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new AuthorConfiguration());
            builder.ApplyConfiguration(new BlogPostConfiguration());
            builder.ApplyConfiguration(new BlogPostTagConfiguration());
            builder.ApplyConfiguration(new BlogPostLocationConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new LocationConfiguration());
            builder.ApplyConfiguration(new TagConfiguration());

            builder.ApplyConfiguration(new CarouselItemConfiguration());
            builder.ApplyConfiguration(new ContentHtmlConfiguration());
            builder.ApplyConfiguration(new ContentTextConfiguration());
            builder.ApplyConfiguration(new FaqConfiguration());
            builder.ApplyConfiguration(new MailingListConfiguration());
            builder.ApplyConfiguration(new ProjectConfiguration());
            builder.ApplyConfiguration(new TestimonialConfiguration());
        }

        public override void BuildQueries(ModelBuilder builder)
        {

        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}
