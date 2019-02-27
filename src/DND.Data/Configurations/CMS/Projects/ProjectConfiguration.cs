using DND.Domain.CMS.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.Configurations.CMS.Projects
{
    public class ProjectConfiguration
           : IEntityTypeConfiguration<Project>
    {

        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowVersion).IsRowVersion();

            builder.Property(p => p.Name)
                 .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DescriptionText)
                .IsRequired()
               .HasMaxLength(200);
        }
    }
}
