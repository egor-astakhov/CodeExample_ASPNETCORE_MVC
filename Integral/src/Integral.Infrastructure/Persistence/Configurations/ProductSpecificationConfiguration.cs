using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integral.Infrastructure.Persistence.Configurations
{
    class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
    {
        public void Configure(EntityTypeBuilder<ProductSpecification> builder)
        {
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Value)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(m => m.LastModifiedBy)
                .HasMaxLength(450);
        }
    }
}
