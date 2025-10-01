using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using ContadorTabaco.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContadorTabaco.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }

    public async Task<List<Order>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Orders.Include(o => o.Product).AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Orders
            .Include(o => o.Product)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
