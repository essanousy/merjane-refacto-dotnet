using Moq;
using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Services.Contracts;
using Refacto.DotNet.Controllers.Services.Implementations.Strategies;

namespace Refacto.DotNet.Controllers.Tests.Services
{
    public class ProductProcessingStrategiesTests
    {
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly NormalProductProcessingStrategy _normalStrategy;
        private readonly SeasonalProductProcessingStrategy _seasonalStrategy;
        private readonly ExpirableProductProcessingStrategy _expirableStrategy;

        public ProductProcessingStrategiesTests()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _normalStrategy = new NormalProductProcessingStrategy(_mockNotificationService.Object);
            _seasonalStrategy = new SeasonalProductProcessingStrategy(_mockNotificationService.Object);
            _expirableStrategy = new ExpirableProductProcessingStrategy(_mockNotificationService.Object);
        }

        [Fact]
        public void NormalStrategy_Should_SendDelayNotification_When_NoStock_And_LeadTimePositive()
        {
            Product product = new()
            {
                LeadTime = 15,
                Available = 0,
                Type = "NORMAL",
                Name = "RJ45 Cable"
            };

            _normalStrategy.Process(product);

            Assert.Equal(0, product.Available);
            Assert.Equal(15, product.LeadTime);
            _mockNotificationService.Verify(service => service.SendDelayNotification(product.LeadTime, product.Name), Times.Once());
        }

        [Fact]
        public void SeasonalStrategy_Should_SendDelayNotification_When_InSeason_And_LeadTimeFits()
        {
            Product product = new()
            {
                LeadTime = 2,
                Available = 5,
                Type = "SEASONAL",
                Name = "Seasonal Item",
                SeasonStartDate = DateTime.UtcNow.Date.AddDays(-1),
                SeasonEndDate = DateTime.UtcNow.Date.AddDays(10)
            };

            _seasonalStrategy.Process(product);

            Assert.Equal(5, product.Available);
            _mockNotificationService.Verify(service => service.SendDelayNotification(product.LeadTime, product.Name), Times.Once());
            _mockNotificationService.Verify(service => service.SendOutOfStockNotification(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void ExpirableStrategy_Should_SendExpirationNotification_And_SetOutOfStock_When_Expired()
        {
            Product product = new()
            {
                LeadTime = 1,
                Available = 4,
                Type = "EXPIRABLE",
                Name = "Milk",
                ExpiryDate = DateTime.UtcNow.Date.AddDays(-1)
            };

            _expirableStrategy.Process(product);

            Assert.Equal(0, product.Available);
            _mockNotificationService.Verify(service => service.SendExpirationNotification(product.Name, product.ExpiryDate!.Value), Times.Once());
        }
    }
}
