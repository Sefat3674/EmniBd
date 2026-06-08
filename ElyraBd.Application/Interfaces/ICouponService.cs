using ElyraBd.Application.DTOs.Coupons;

namespace ElyraBd.Application.Interfaces;

public interface ICouponService
{
    Task<CouponValidationDto> ValidateAsync(string code, decimal subTotal, CancellationToken cancellationToken = default);
}
