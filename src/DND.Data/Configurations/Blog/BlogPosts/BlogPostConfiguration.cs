using DND.Domain.Blog.BlogPosts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.Blog.BlogPosts
{
    public class BlogPostConfiguration
           : IEntityTypeConfiguration<BlogPost>
    {
        public void Configure(EntityTypeBuilder<BlogPost> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.Property(p => p.Title)
                 .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.ShortDescription)
                .IsRequired()
               .HasMaxLength(5000);

            builder.Property(p => p.Description)
              .IsRequired()
             .HasMaxLength(300000);

            builder.Property(p => p.UrlSlug)
             .IsRequired()
            .HasMaxLength(200);

            builder.Property(p => p.CarouselText)
            .HasMaxLength(200);

            builder.Property(p => p.ShowInCarousel)
            .IsRequired();

            builder.Property(p => p.Published)
            .IsRequired();

            builder.Property(p => p.CreatedOn)
           .IsRequired();

            builder.HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasMany(p => p.Tags)
           .WithOne().IsRequired()
           .HasForeignKey(p => p.BlogPostId);

            builder.HasMany(p => p.Locations)
            .WithOne().IsRequired()
           .HasForeignKey(p => p.BlogPostId);

            builder.Property(p => p.ShowLocationDetail)
            .IsRequired();

            builder.Property(p => p.ShowLocationMap)
            .IsRequired();

            builder.Property(p => p.MapHeight)
            .IsRequired();

            builder.Property(p => p.MapZoom)
            .IsRequired();

            builder.Property(p => p.Album)
            .IsRequired();

            builder.Property(p => p.ShowPhotosInAlbum)
            .IsRequired();
        }
    }
}
