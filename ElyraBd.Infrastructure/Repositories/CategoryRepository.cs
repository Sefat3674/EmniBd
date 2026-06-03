using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;
using ElyraBd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElyraBd.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetActiveWithProductCountAsync(
        CancellationToken cancellationToken = default) =>
        await DbSet
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Include(c => c.Products.Where(p => p.IsActive))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
}
