using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Integral.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.ShortDescription)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.ImageName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.ImagePath)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(m => m.ApplicationArea)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.Features)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(m => m.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(m => m.LastModifiedBy)
                .HasMaxLength(450);

            builder.Property(m => m.SortingOrder)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
