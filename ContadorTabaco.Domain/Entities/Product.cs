namespace ContadorTabaco.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Decimal Price { get; set; }
}
