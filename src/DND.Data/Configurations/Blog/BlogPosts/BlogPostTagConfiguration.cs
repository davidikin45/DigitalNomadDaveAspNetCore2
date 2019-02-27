using DND.Domain.Blog.BlogPosts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.BlogPosts
{
    public class BlogPostTagConfiguration
           : IEntityTypeConfiguration<BlogPostTag>
    {

        public void Configure(EntityTypeBuilder<BlogPostTag> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Tag)
                .WithMany()
                .HasForeignKey(p => p.TagId);
        }
    }
}
