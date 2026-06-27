using Refacto.DotNet.Controllers.Entities;

namespace Refacto.DotNet.Controllers.Database.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdWithItemsAsync(long orderId, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}