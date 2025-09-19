using ContadorTabaco.Domain.Entities;

namespace ContadorTabaco.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
}
