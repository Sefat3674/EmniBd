using ElyraBd.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElyraBd.Infrastructure.Persistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Description).HasMaxLength(500);
        builder.Property(a => a.IpAddress).HasMaxLength(45);

        builder.HasOne(a => a.User)
            .WithMany(u => u.Activities)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.CreatedAt);
        builder.HasIndex(a => a.ActivityType);
    }
}
