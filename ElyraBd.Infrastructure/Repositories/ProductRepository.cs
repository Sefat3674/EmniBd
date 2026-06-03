using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;
using ElyraBd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        await DbSet
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Offers)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> GetOnSaleAsync(int count, CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive && p.DiscountPercent > 0)
            .OrderByDescending(p => p.DiscountPercent)
            .Take(count)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> SearchAsync(
        string? term,
        int? categoryId,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim().ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(t) ||
                p.Description.ToLower().Contains(t));
        }

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
}
