namespace Refacto.DotNet.Controllers.Services.Contracts
{
    public interface IOrderProcessingService
    {
        Task<Entities.Order?> ProcessAsync(long orderId, CancellationToken cancellationToken = default); 
    }
}
