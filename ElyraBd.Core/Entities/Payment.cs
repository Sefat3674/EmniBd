using ElyraBd.Core.Common;
using ElyraBd.Core.Enums;

namespace ElyraBd.Core.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public DateTime? PaidAt { get; set; }
}
