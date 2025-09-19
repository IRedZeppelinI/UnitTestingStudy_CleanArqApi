namespace ContadorTabaco.Application.Features.Products.Dtos;

public class ProductDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Decimal Price { get; set; }

    //public List<OrderDto> Orders { get; set; } = new();
}
