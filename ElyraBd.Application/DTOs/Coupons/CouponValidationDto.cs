namespace ElyraBd.Application.DTOs.Coupons;

public class CouponValidationDto
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal DiscountAmount { get; set; }
    public string? Code { get; set; }
}
