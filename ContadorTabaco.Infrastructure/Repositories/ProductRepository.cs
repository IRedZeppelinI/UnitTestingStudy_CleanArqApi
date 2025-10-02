using ContadorTabaco.Application.Interfaces;
using ContadorTabaco.Domain.Entities;
using ContadorTabaco.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContadorTabaco.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext appDbContext)
    {
        _context = appDbContext;
    }
        

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Products.FindAsync(new object[] { id}, cancellationToken);
    }
    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }
}
