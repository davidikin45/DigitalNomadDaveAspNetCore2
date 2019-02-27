using DND.Domain.CMS.Faqs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.Faqs
{
    public class FaqConfiguration
           : IEntityTypeConfiguration<Faq>
    {
        public void Configure(EntityTypeBuilder<Faq> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.Property(p => p.Question)
                 .IsRequired();

            builder.Property(p => p.Answer)
                .IsRequired();
        }
    }
}
