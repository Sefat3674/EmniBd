using ElyraBd.Core.Entities;

namespace ElyraBd.Core.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetActiveWithProductCountAsync(CancellationToken cancellationToken = default);
}
