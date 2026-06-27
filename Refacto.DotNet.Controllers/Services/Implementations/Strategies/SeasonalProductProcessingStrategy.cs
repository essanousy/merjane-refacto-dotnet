using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Services.Contracts;

namespace Refacto.DotNet.Controllers.Services.Implementations.Strategies
{
    public class SeasonalProductProcessingStrategy : IProductProcessingStrategy
    {
        private readonly INotificationService _notificationService;

        public string ProductType => "SEASONAL";

        public SeasonalProductProcessingStrategy(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Process(Product product)
        {
            DateTime today = DateTime.UtcNow.Date;

            if (!product.SeasonStartDate.HasValue || !product.SeasonEndDate.HasValue)
            {
                _notificationService.SendOutOfStockNotification(product.Name);
                product.Available = 0;
                return;
            }

            DateTime start = product.SeasonStartDate.Value.Date;
            DateTime end = product.SeasonEndDate.Value.Date;

            if (today < start || today > end)
            {
                _notificationService.SendOutOfStockNotification(product.Name);
                product.Available = 0;
                return;
            }

            if (today.AddDays(product.LeadTime) > end)
            {
                _notificationService.SendOutOfStockNotification(product.Name);
                product.Available = 0;
                return;
            }

            _notificationService.SendDelayNotification(product.LeadTime, product.Name);
        }
    }
}