using Microsoft.EntityFrameworkCore;
using Refacto.DotNet.Controllers.Database.Context;
using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Database.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _ctx;

        public OrderRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public Task<Order?> GetByIdWithItemsAsync(long orderId, CancellationToken cancellationToken = default)
        {
            return _ctx.Orders
                .Include(o => o.Items)
                .SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _ = await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}