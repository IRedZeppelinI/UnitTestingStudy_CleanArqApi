using ContadorTabaco.Domain.Entities;

namespace ContadorTabaco.Application.Features.Orders.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Decimal TotalCost { get; set; }
    public DateTime OrderDate { get; set; }
}

