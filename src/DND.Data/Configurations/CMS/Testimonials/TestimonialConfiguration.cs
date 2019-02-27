using DND.Domain.CMS.Testimonials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.Testimonials
{
    public class TestimonialConfiguration
           : IEntityTypeConfiguration<Testimonial>
    {
        public void Configure(EntityTypeBuilder<Testimonial> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion(); // Only works for byte array. Will create a timestamp in EF6, rowversion EFCore
            //Property(p => p.RowVersion).IsConcurrencyToken().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed); //works for int

            builder.Property(p => p.Source)
                 .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.QuoteText)
                .IsRequired()
               .HasMaxLength(5000);
        }
    }
}
