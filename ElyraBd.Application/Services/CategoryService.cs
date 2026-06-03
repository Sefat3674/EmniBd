using AutoMapper;
using ElyraBd.Application.DTOs.Categories;
using ElyraBd.Application.Interfaces;
using ElyraBd.Core.Entities;
using ElyraBd.Core.Interfaces;

namespace ElyraBd.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Categories.GetActiveWithProductCountAsync(cancellationToken);
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            IsActive = c.IsActive,
            ProductCount = c.Products.Count
        }).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        return category is null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<int> CreateAsync(CategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Category>(dto);
        entity.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Categories.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(CategoryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Categories.GetByIdAsync(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        await _unitOfWork.Categories.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");
        await _unitOfWork.Categories.DeleteAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
