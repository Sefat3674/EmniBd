using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;
using ElyraBd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Repositories;

public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        await DbSet.FirstOrDefaultAsync(
            c => c.Code == code.Trim().ToUpperInvariant(),
            cancellationToken);
}
