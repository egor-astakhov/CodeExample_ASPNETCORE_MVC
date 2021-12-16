using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integral.Infrastructure.Persistence.Configurations
{
    public class ApplicationSettingConfiguration : IEntityTypeConfiguration<ApplicationSetting>
    {
        public void Configure(EntityTypeBuilder<ApplicationSetting> builder)
        {
            builder.HasKey(t => t.Key);

            builder.Property(t => t.Key)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(t => t.Value)
                .IsRequired();
        }
    }
}
