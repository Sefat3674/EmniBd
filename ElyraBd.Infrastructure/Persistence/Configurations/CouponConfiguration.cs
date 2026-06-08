using ElyraBd.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElyraBd.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("CouponId");

        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Description).HasMaxLength(300);
        builder.Property(c => c.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(c => c.DiscountPercent).HasColumnType("decimal(5,2)");
        builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");

        builder.HasIndex(c => c.Code).IsUnique();
    }
}
