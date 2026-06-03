using ElyraBd.Application.DTOs.Categories;

namespace ElyraBd.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryDto>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CategoryDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(CategoryDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
