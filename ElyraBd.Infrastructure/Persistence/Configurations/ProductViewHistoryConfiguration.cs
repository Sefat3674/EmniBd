using ElyraBd.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElyraBd.Infrastructure.Persistence.Configurations;

public class ProductViewHistoryConfiguration : IEntityTypeConfiguration<ProductViewHistory>
{
    public void Configure(EntityTypeBuilder<ProductViewHistory> builder)
    {
        builder.ToTable("ProductViewHistories");
        builder.HasKey(v => v.Id);

        builder.HasOne(v => v.User)
            .WithMany(u => u.ProductViews)
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Product)
            .WithMany(p => p.ViewHistories)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => new { v.UserId, v.ProductId });
        builder.HasIndex(v => v.ViewedAt);
    }
}
