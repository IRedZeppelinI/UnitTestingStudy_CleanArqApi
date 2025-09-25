namespace ContadorTabaco.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Decimal TotalCost { get; private set; }
    public DateTime OrderDate { get; set; }

    private Order() { }

    public static Order Create(Product product, int quantity)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product), "Produto não pode ser nulo.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantidade deve ser positiva.", nameof(quantity));
        }

        var order = new Order
        {
            Product = product,
            ProductId = product.Id,
            Quantity = quantity,
            OrderDate = DateTime.UtcNow // Ou DateTime.Now, dependendo do requisito
        };

        order.CalculateTotalCost();

        return order;
    }

    public static Order Create(Product product, int quantity, DateTime orderDate)
    {
        // Chama a lógica de criação principal
        var order = Create(product, quantity);

        // E depois define a data específica
        order.OrderDate = orderDate;

        return order;
    }

    private void CalculateTotalCost()
    {
        
        TotalCost = Product.Price * Quantity;
    }

}
