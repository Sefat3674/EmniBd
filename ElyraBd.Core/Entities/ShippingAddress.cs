using ElyraBd.Core.Common;

namespace ElyraBd.Core.Entities;

public class ShippingAddress : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    /// <summary>Set when this address is used for a specific order (Order → ShippingAddress 1:1).</summary>
    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = "Bangladesh";
}
