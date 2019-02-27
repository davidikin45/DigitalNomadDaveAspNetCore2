using DND.Domain.CMS.ContentTexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.ContentTexts
{
    public class ContentTextConfiguration
           : IEntityTypeConfiguration<ContentText>
    {
        public void Configure(EntityTypeBuilder<ContentText> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
