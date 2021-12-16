using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integral.Infrastructure.Persistence.Configurations
{
    class ProductAttachmentConfiguration : IEntityTypeConfiguration<ProductAttachment>
    {
        public void Configure(EntityTypeBuilder<ProductAttachment> builder)
        {
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.FileVersion)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(m => m.FileName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.FilePath)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(m => m.LastModifiedBy)
                .HasMaxLength(450);
        }
    }
}
