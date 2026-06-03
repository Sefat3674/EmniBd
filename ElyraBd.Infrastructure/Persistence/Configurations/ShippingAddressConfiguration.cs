using ElyraBd.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElyraBd.Infrastructure.Persistence.Configurations;

public class ShippingAddressConfiguration : IEntityTypeConfiguration<ShippingAddress>
{
    public void Configure(EntityTypeBuilder<ShippingAddress> builder)
    {
        builder.ToTable("ShippingAddresses");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("AddressId");

        builder.Property(s => s.FullName).IsRequired().HasMaxLength(150);
        builder.Property(s => s.Phone).IsRequired().HasMaxLength(20);
        builder.Property(s => s.AddressLine).IsRequired().HasMaxLength(300);
        builder.Property(s => s.City).IsRequired().HasMaxLength(100);
        builder.Property(s => s.PostalCode).IsRequired().HasMaxLength(20);
        builder.Property(s => s.Country).IsRequired().HasMaxLength(100);

        builder.HasOne(s => s.User)
            .WithMany(u => u.ShippingAddresses)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Order)
            .WithOne(o => o.ShippingAddress)
            .HasForeignKey<ShippingAddress>(s => s.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.OrderId).IsUnique().HasFilter("[OrderId] IS NOT NULL");
    }
}
