using DND.Domain.CMS.ContentHtmls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.ContentHtmls
{
    public class ContentHtmlConfiguration
           : IEntityTypeConfiguration<ContentHtml>
    {
        public void Configure(EntityTypeBuilder<ContentHtml> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Ignore(p => p.Deleted);
            builder.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
