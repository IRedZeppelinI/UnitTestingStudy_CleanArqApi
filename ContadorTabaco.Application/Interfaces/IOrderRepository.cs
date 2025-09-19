using ContadorTabaco.Domain.Entities;

namespace ContadorTabaco.Application.Interfaces;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(CancellationToken cancellationToken);
}
