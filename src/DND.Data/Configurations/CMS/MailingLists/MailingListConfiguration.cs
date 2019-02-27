using DND.Domain.CMS.MailingLists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.MailingLists
{
    public class MailingListConfiguration
           : IEntityTypeConfiguration<MailingList>
    {
        public void Configure(EntityTypeBuilder<MailingList> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.Property(p => p.Email)
                 .IsRequired();
        }
    }
}
