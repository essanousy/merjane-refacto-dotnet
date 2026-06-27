using Moq;
using Refacto.DotNet.Controllers.Database.Repositories;
using Refacto.DotNet.Controllers.Entities;
using Refacto.DotNet.Controllers.Services.Contracts;
using Refacto.DotNet.Controllers.Services.Impl;

namespace Refacto.DotNet.Controllers.Tests.Services
{
    public class OrderProcessingServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IProductProcessingStrategy> _mockNormalStrategy;
        private readonly OrderProcessingService _service;

        public OrderProcessingServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockNormalStrategy = new Mock<IProductProcessingStrategy>();
            _mockNormalStrategy.SetupGet(s => s.ProductType).Returns("NORMAL");

            _service = new OrderProcessingService(
                _mockOrderRepository.Object,
                [_mockNormalStrategy.Object]);
        }

        [Fact]
        public async Task ProcessAsync_Should_ReturnNull_When_OrderDoesNotExist()
        {
            long orderId = 123;
            _mockOrderRepository
                .Setup(repo => repo.GetByIdWithItemsAsync(orderId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Order?)null);

            Order? result = await _service.ProcessAsync(orderId);

            Assert.Null(result);
            _mockOrderRepository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            _mockNormalStrategy.Verify(strategy => strategy.Process(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_Should_ProcessOnlyMatchingStrategies_And_SaveChanges()
        {
            Product normalProduct = new() { Type = "normal", Name = "USB Cable" };
            Product unknownProduct = new() { Type = "UNKNOWN", Name = "Unknown Product" };
            Product nullTypeProduct = new() { Type = null, Name = "No Type Product" };
            Order order = new() { Items = [normalProduct, unknownProduct, nullTypeProduct] };

            _mockOrderRepository
                .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            Order? result = await _service.ProcessAsync(order.Id);

            Assert.Same(order, result);
            _mockNormalStrategy.Verify(strategy => strategy.Process(normalProduct), Times.Once);
            _mockNormalStrategy.Verify(strategy => strategy.Process(It.Is<Product>(p => p != normalProduct)), Times.Never);
            _mockOrderRepository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_Should_SaveChanges_When_OrderHasNoItems()
        {
            Order order = new() { Items = null };

            _mockOrderRepository
                .Setup(repo => repo.GetByIdWithItemsAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            Order? result = await _service.ProcessAsync(order.Id);

            Assert.Same(order, result);
            _mockNormalStrategy.Verify(strategy => strategy.Process(It.IsAny<Product>()), Times.Never);
            _mockOrderRepository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}