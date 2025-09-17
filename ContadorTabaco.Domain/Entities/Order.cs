namespace ContadorTabaco.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public Decimal TotalCost { get; set; }
    public DateTime OrderDate { get; set; }

}
