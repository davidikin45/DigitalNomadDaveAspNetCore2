using DND.Domain.Blog.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.Authors
{
    public class AuthorConfiguration
           : IEntityTypeConfiguration<Author>
    {

        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.RowVersion).IsRowVersion();
            builder.Property(p => p.Name).IsRequired();
            builder.Property(p => p.UrlSlug).HasMaxLength(50);
        }
    }
}
