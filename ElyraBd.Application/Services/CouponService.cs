using ElyraBd.Application.DTOs.Coupons;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Interfaces;

namespace ElyraBd.Application.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _unitOfWork;

    public CouponService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<CouponValidationDto> ValidateAsync(string code, decimal subTotal, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return new CouponValidationDto { IsValid = false, Message = "Coupon code is required." };
        }

        var coupon = await _unitOfWork.Coupons.GetByCodeAsync(code.Trim(), cancellationToken);
        if (coupon is null)
        {
            return new CouponValidationDto { IsValid = false, Message = "Invalid coupon code." };
        }

        if (!coupon.IsActive)
        {
            return new CouponValidationDto { IsValid = false, Message = "This coupon is no longer active." };
        }

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value < DateTime.UtcNow)
        {
            return new CouponValidationDto { IsValid = false, Message = "This coupon has expired." };
        }

        if (coupon.MaxUses > 0 && coupon.UsedCount >= coupon.MaxUses)
        {
            return new CouponValidationDto { IsValid = false, Message = "This coupon has reached its usage limit." };
        }

        if (subTotal < coupon.MinOrderAmount)
        {
            return new CouponValidationDto
            {
                IsValid = false,
                Message = $"Minimum order amount is ৳{coupon.MinOrderAmount:N0} to use this coupon."
            };
        }

        var discount = coupon.DiscountPercent > 0
            ? Math.Round(subTotal * coupon.DiscountPercent / 100m, 2)
            : coupon.DiscountAmount;

        discount = Math.Min(discount, subTotal);

        return new CouponValidationDto
        {
            IsValid = true,
            Message = "Coupon applied successfully.",
            DiscountAmount = discount,
            Code = coupon.Code
        };
    }
}
