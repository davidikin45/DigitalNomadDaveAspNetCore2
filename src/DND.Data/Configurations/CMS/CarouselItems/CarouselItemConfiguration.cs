using DND.Domain.CMS.CarouselItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.CarouselItems
{
    public class CarouselItemConfiguration
           : IEntityTypeConfiguration<CarouselItem>
    {
        public void Configure(EntityTypeBuilder<CarouselItem> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.Property(p => p.CarouselText)
                .HasMaxLength(200);

            builder.Property(p => p.OpenInNewWindow)
                .IsRequired();

            builder.Property(p => p.Published)
              .IsRequired();
        }
    }
}
