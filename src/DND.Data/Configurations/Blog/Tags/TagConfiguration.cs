using DND.Domain.Blog.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.Tags
{
    public class TagConfiguration
           : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
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
