using ElyraBd.Application.DTOs.Products;

namespace ElyraBd.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetOnSaleAsync(int count, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> SearchAsync(string? term, int? categoryId, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDetailDto?> GetDetailForCustomerAsync(int id, string? userId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(ProductUpsertDto dto, IList<Stream> imageStreams, IList<string> fileNames, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProductUpsertDto dto, CancellationToken cancellationToken = default);
    Task AddImagesAsync(int productId, IList<Stream> imageStreams, IList<string> fileNames, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task ToggleActiveAsync(int id, CancellationToken cancellationToken = default);
}
