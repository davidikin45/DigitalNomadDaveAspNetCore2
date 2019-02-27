using DND.Domain.Blog.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.Categories
{
    public class CategoryConfiguration
           : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.Property(p => p.Name)
                 .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.UrlSlug)
                .IsRequired()
               .HasMaxLength(50);

            builder.Property(p => p.Description)
              .IsRequired()
             .HasMaxLength(200);
        }
    }
}
