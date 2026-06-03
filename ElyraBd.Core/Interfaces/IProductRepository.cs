using ElyraBd.Core.Entities;

namespace ElyraBd.Core.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetOnSaleAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchAsync(string? term, int? categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
}
