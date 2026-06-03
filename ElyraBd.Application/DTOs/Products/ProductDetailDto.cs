namespace ElyraBd.Application.DTOs.Products;

public class ProductDetailDto : ProductDto
{
    public IList<string> ImageUrls { get; set; } = new List<string>();
    public IList<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
}

public class ReviewDto
{
    public string UserName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
