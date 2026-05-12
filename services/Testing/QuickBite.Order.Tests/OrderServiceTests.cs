using Microsoft.Extensions.Logging;
using Moq;
using QuickBite.Order.Application.DTOs;
using QuickBite.Order.Application.Interfaces;
using QuickBite.Order.Application.Services;
using QuickBite.Order.Domain.Entities;
using QuickBite.Order.Domain.Enums;

namespace QuickBite.Order.Tests;

[TestClass]
public class OrderServiceTests
{
    private Mock<IOrderRepository>? _orderRepositoryMock;
    private Mock<IPaymentGateway>? _paymentGatewayMock;
    private Mock<IDeliveryDispatcher>? _deliveryDispatcherMock;
    private Mock<ILogger<OrderService>>? _loggerMock;
    private OrderService? _orderService;

    [TestInitialize]
    public void Setup()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _paymentGatewayMock = new Mock<IPaymentGateway>();
        _deliveryDispatcherMock = new Mock<IDeliveryDispatcher>();
        _loggerMock = new Mock<ILogger<OrderService>>();

        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _paymentGatewayMock.Object,
            _deliveryDispatcherMock.Object,
            _loggerMock.Object);
    }

    [TestMethod]
    public async Task PlaceOrderAsync_ValidRequest_ReturnsOrderResponse()
    {
        // Arrange
        var request = new PlaceOrderRequestDto
        {
            CustomerId = Guid.NewGuid(),
            RestaurantId = Guid.NewGuid(),
            Items = new List<PlaceOrderItemDto>
            {
                new() { MenuItemId = Guid.NewGuid(), Name = "Test Item", Price = 10.0m, Quantity = 2 }
            },
            DeliveryAddress = "123 Test St",
            ModeOfPayment = PaymentMode.CASH_ON_DELIVERY,
            Discount = 0m
        };

        var order = new Domain.Entities.Order
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            RestaurantId = request.RestaurantId,
            TotalAmount = 20.0m,
            Discount = 0m,
            FinalAmount = 20.0m,
            ModeOfPayment = request.ModeOfPayment,
            OrderStatus = OrderStatus.CONFIRMED,
            OrderDate = DateTime.UtcNow,
            EstimatedDelivery = DateTime.UtcNow.AddMinutes(45),
            DeliveryAddress = request.DeliveryAddress
        };

        _orderRepositoryMock!.Setup(r => r.AddOrderAsync(It.IsAny<Domain.Entities.Order>())).Returns(Task.CompletedTask);
        _orderRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        _paymentGatewayMock!.Setup(p => p.ProcessPaymentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<decimal>(), It.IsAny<PaymentMode>())).ReturnsAsync(true);
        _deliveryDispatcherMock!.Setup(d => d.AssignAgentAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>())).ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _orderService.PlaceOrderAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(request.CustomerId, result.CustomerId);
        Assert.AreEqual(request.RestaurantId, result.RestaurantId);
        Assert.AreEqual(20.0m, result.TotalAmount);
        Assert.AreEqual(20.0m, result.FinalAmount);
    }

    [TestMethod]
    public void PlaceOrderAsync_NoItems_ThrowsException()
    {
        // Arrange
        var request = new PlaceOrderRequestDto
        {
            CustomerId = Guid.NewGuid(),
            RestaurantId = Guid.NewGuid(),
            Items = new List<PlaceOrderItemDto>(),
            DeliveryAddress = "123 Test St",
            ModeOfPayment = PaymentMode.CASH_ON_DELIVERY,
            Discount = 0m
        };

        // Act & Assert
        try
        {
            _orderService!.PlaceOrderAsync(request).GetAwaiter().GetResult();
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Cannot place an order with no items.", ex.Message);
        }
    }

    [TestMethod]
    public async Task GetOrderByIdAsync_OrderExists_ReturnsOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Domain.Entities.Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            RestaurantId = Guid.NewGuid(),
            TotalAmount = 20.0m,
            Discount = 0m,
            FinalAmount = 20.0m,
            ModeOfPayment = PaymentMode.CASH_ON_DELIVERY,
            OrderStatus = OrderStatus.PLACED,
            OrderDate = DateTime.UtcNow,
            EstimatedDelivery = DateTime.UtcNow.AddMinutes(45),
            DeliveryAddress = "123 Test St"
        };

        _orderRepositoryMock!.Setup(r => r.FindByOrderIdAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _orderService!.GetOrderByIdAsync(orderId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(orderId, result.OrderId);
    }

    [TestMethod]
    public async Task GetOrderByIdAsync_OrderNotFound_ReturnsNull()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _orderRepositoryMock!.Setup(r => r.FindByOrderIdAsync(orderId)).ReturnsAsync((Domain.Entities.Order?)null);

        // Act
        var result = await _orderService!.GetOrderByIdAsync(orderId);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetOrdersByCustomerAsync_ReturnsOrders()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orders = new List<Domain.Entities.Order>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                RestaurantId = Guid.NewGuid(),
                TotalAmount = 20.0m,
                Discount = 0m,
                FinalAmount = 20.0m,
                ModeOfPayment = PaymentMode.CASH_ON_DELIVERY,
                OrderStatus = OrderStatus.PLACED,
                OrderDate = DateTime.UtcNow,
                EstimatedDelivery = DateTime.UtcNow.AddMinutes(45),
                DeliveryAddress = "123 Test St"
            }
        };

        _orderRepositoryMock!.Setup(r => r.FindByCustomerIdAsync(customerId)).ReturnsAsync(orders);

        // Act
        var result = await _orderService!.GetOrdersByCustomerAsync(customerId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(customerId, result[0].CustomerId);
    }

    [TestMethod]
    public async Task UpdateStatusAsync_OrderExists_UpdatesStatus()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Domain.Entities.Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            RestaurantId = Guid.NewGuid(),
            TotalAmount = 20.0m,
            Discount = 0m,
            FinalAmount = 20.0m,
            ModeOfPayment = PaymentMode.CASH_ON_DELIVERY,
            OrderStatus = OrderStatus.PLACED,
            OrderDate = DateTime.UtcNow,
            EstimatedDelivery = DateTime.UtcNow.AddMinutes(45),
            DeliveryAddress = "123 Test St"
        };

        var request = new UpdateStatusRequestDto { NewStatus = OrderStatus.CONFIRMED };

        _orderRepositoryMock!.Setup(r => r.FindByOrderIdAsync(orderId)).ReturnsAsync(order);
        _orderRepositoryMock.Setup(r => r.UpdateOrder(It.IsAny<Domain.Entities.Order>()));
        _orderRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _orderService!.UpdateStatusAsync(orderId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(orderId, result.OrderId);
        _orderRepositoryMock.Verify(r => r.UpdateOrder(It.IsAny<Domain.Entities.Order>()), Times.Once);
    }

    [TestMethod]
    public async Task CancelOrderAsync_OrderExists_CancelsOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Domain.Entities.Order
        {
            Id = orderId,
            CustomerId = Guid.NewGuid(),
            RestaurantId = Guid.NewGuid(),
            TotalAmount = 20.0m,
            Discount = 0m,
            FinalAmount = 20.0m,
            ModeOfPayment = PaymentMode.CASH_ON_DELIVERY,
            OrderStatus = OrderStatus.PLACED,
            OrderDate = DateTime.UtcNow,
            EstimatedDelivery = DateTime.UtcNow.AddMinutes(45),
            DeliveryAddress = "123 Test St"
        };

        var request = new CancelOrderRequestDto { Reason = "Customer request" };

        _orderRepositoryMock!.Setup(r => r.FindByOrderIdAsync(orderId)).ReturnsAsync(order);
        _orderRepositoryMock.Setup(r => r.UpdateOrder(It.IsAny<Domain.Entities.Order>()));
        _orderRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _orderService!.CancelOrderAsync(orderId, request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(OrderStatus.CANCELLED, result.OrderStatus);
        _orderRepositoryMock.Verify(r => r.UpdateOrder(It.IsAny<Domain.Entities.Order>()), Times.Once);
    }
}