using ElyraBd.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElyraBd.Infrastructure.Persistence.Configurations;

public class ProductOfferConfiguration : IEntityTypeConfiguration<ProductOffer>
{
    public void Configure(EntityTypeBuilder<ProductOffer> builder)
    {
        builder.ToTable("ProductOffers");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title).HasMaxLength(150);
        builder.Property(o => o.DiscountPercent).HasColumnType("decimal(5,2)");

        builder.HasOne(o => o.Product)
            .WithMany(p => p.Offers)
            .HasForeignKey(o => o.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
