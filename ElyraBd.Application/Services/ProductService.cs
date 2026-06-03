using AutoMapper;
using ElyraBd.Application.DTOs.Products;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;

namespace ElyraBd.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;
    private readonly IMapper _mapper;
    private readonly IActivityTrackingService _activityTracking;

    public ProductService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorage,
        IMapper mapper,
        IActivityTrackingService activityTracking)
    {
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
        _mapper = mapper;
        _activityTracking = activityTracking;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllForAdminAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllWithDetailsAsync(cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<IReadOnlyList<ProductDto>> GetFeaturedAsync(int count, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetFeaturedAsync(count, cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<IReadOnlyList<ProductDto>> GetOnSaleAsync(int count, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetOnSaleAsync(count, cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<IReadOnlyList<ProductDto>> SearchAsync(
        string? term,
        int? categoryId,
        CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.SearchAsync(term, categoryId, cancellationToken);
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDetailDto?> GetDetailAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id, cancellationToken);
        return product is null ? null : _mapper.Map<ProductDetailDto>(product);
    }

    public async Task<ProductDetailDto?> GetDetailForCustomerAsync(
        int id,
        string? userId,
        CancellationToken cancellationToken = default)
    {
        var detail = await GetDetailAsync(id, cancellationToken);
        if (detail is not null && !string.IsNullOrEmpty(userId))
            await _activityTracking.TrackProductViewAsync(userId, id, cancellationToken);
        return detail;
    }

    public async Task<int> CreateAsync(
        ProductUpsertDto dto,
        IList<Stream> imageStreams,
        IList<string> fileNames,
        CancellationToken cancellationToken = default)
    {
        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SaveImagesAsync(product.Id, imageStreams, fileNames, cancellationToken);
        return product.Id;
    }

    public async Task UpdateAsync(ProductUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        _mapper.Map(dto, product);
        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddImagesAsync(
        int productId,
        IList<Stream> imageStreams,
        IList<string> fileNames,
        CancellationToken cancellationToken = default)
    {
        await SaveImagesAsync(productId, imageStreams, fileNames, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        foreach (var img in product.Images)
            _fileStorage.DeleteFile(img.ImageUrl);

        await _unitOfWork.Products.DeleteAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ToggleActiveAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");
        product.IsActive = !product.IsActive;
        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task SaveImagesAsync(
        int productId,
        IList<Stream> imageStreams,
        IList<string> fileNames,
        CancellationToken cancellationToken)
    {
        var imageRepo = _unitOfWork.Repository<ProductImage>();
        var order = 0;
        for (var i = 0; i < imageStreams.Count; i++)
        {
            var url = await _fileStorage.SaveProductImageAsync(imageStreams[i], fileNames[i], cancellationToken);
            await imageRepo.AddAsync(new ProductImage
            {
                ProductId = productId,
                ImageUrl = url,
                IsPrimary = order == 0,
                DisplayOrder = order++
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
