using AutoMapper;
using ElyraBd.Application.DTOs.Carts;
using ElyraBd.Application.DTOs.Categories;
using ElyraBd.Application.DTOs.Notifications;
using ElyraBd.Application.DTOs.Orders;
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

        CreateMap<Cart, CartDto>()
            .ForMember(d => d.TotalItems, o => o.MapFrom(s => s.Items.Sum(i => i.Quantity)))
            .ForMember(d => d.SubTotal, o => o.MapFrom(s => s.Items.Sum(i => i.Quantity * i.UnitPrice)));

        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
            .ForMember(d => d.ProductImageUrl, o => o.MapFrom(s =>
                s.Product.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Quantity * s.UnitPrice));

        CreateMap<Order, OrderSummaryDto>()
            .ForMember(d => d.ItemCount, o => o.MapFrom(s => s.Items.Sum(i => i.Quantity)));

        CreateMap<Order, OrderDetailDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.FullName : s.User.FullName))
            .ForMember(d => d.CustomerEmail, o => o.MapFrom(s => s.User.Email ?? string.Empty))
            .ForMember(d => d.CustomerPhone, o => o.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.Phone : (s.User.PhoneNumber ?? string.Empty)))
            .ForMember(d => d.AddressLine, o => o.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.AddressLine : string.Empty))
            .ForMember(d => d.City, o => o.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.City : string.Empty))
            .ForMember(d => d.PostalCode, o => o.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.PostalCode : string.Empty))
            .ForMember(d => d.Country, o => o.MapFrom(s => s.ShippingAddress != null ? s.ShippingAddress.Country : string.Empty))
            .ForMember(d => d.PaymentMethod, o => o.MapFrom(s => s.Payment != null ? s.Payment.PaymentMethod : Core.Enums.PaymentMethod.CashOnDelivery))
            .ForMember(d => d.Items, o => o.MapFrom(s => s.Items))
            .ForMember(d => d.Timeline, o => o.Ignore());

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.ProductImageUrl, o => o.MapFrom(s =>
                s.Product.Images.OrderByDescending(i => i.IsPrimary).ThenBy(i => i.DisplayOrder)
                    .Select(i => i.ImageUrl).FirstOrDefault()))
            .ForMember(d => d.LineTotal, o => o.MapFrom(s => s.Quantity * s.UnitPrice));

        CreateMap<Notification, NotificationDto>();
    }
}
