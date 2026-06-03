using Microsoft.AspNetCore.Identity;

namespace ElyraBd.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ShippingAddress> ShippingAddresses { get; set; } = new List<ShippingAddress>();
    public ICollection<UserActivity> Activities { get; set; } = new List<UserActivity>();
    public ICollection<ProductViewHistory> ProductViews { get; set; } = new List<ProductViewHistory>();
}
