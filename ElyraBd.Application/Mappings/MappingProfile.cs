using AutoMapper;
using ElyraBd.Application.DTOs.Categories;
using ElyraBd.Application.DTOs.Products;
using ElyraBd.Core.Entities;

namespace ElyraBd.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>()
            .ForMember(d => d.ProductCount, o => o.MapFrom(s => s.Products.Count(p => p.IsActive)));

        CreateMap<CategoryDto, Category>()
            .ForMember(d => d.Products, o => o.Ignore());

        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name))
            .ForMember(d => d.SalePrice, o => o.MapFrom(s => s.SalePrice))
            .ForMember(d => d.HasDiscount, o => o.MapFrom(s => s.HasDiscount))
            .ForMember(d => d.PrimaryImageUrl, o => o.MapFrom(s =>
                s.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.AverageRating, o => o.MapFrom(s =>
                s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0))
            .ForMember(d => d.ReviewCount, o => o.MapFrom(s => s.Reviews.Count));

        CreateMap<Product, ProductDetailDto>()
            .IncludeBase<Product, ProductDto>()
            .ForMember(d => d.ImageUrls, o => o.MapFrom(s =>
                s.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()))
            .ForMember(d => d.Reviews, o => o.MapFrom(s => s.Reviews
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    UserName = r.User.FullName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })));

        CreateMap<ProductUpsertDto, Product>()
            .ForMember(d => d.Images, o => o.Ignore())
            .ForMember(d => d.Category, o => o.Ignore());
    }
}
