using ElyraBd.Core.Common;
using ElyraBd.Core.Enums;

namespace ElyraBd.Core.Entities;

public class Order : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public ShippingAddress? ShippingAddress { get; set; }
    public Payment? Payment { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
