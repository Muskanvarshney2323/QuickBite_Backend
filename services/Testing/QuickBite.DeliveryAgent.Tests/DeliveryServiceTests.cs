using Moq;
using QuickBite.DeliveryAgent.Application.DTOs;
using QuickBite.DeliveryAgent.Application.Interfaces;
using QuickBite.DeliveryAgent.Application.Services;
using QuickBite.DeliveryAgent.Domain.Enums;

namespace QuickBite.DeliveryAgent.Tests;

/// <summary>
/// Unit tests for delivery agent service operations.
/// </summary>
[TestClass]
public class DeliveryServiceTests
{
    private Mock<IDeliveryRepository>? _mockRepository;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRepository = new Mock<IDeliveryRepository>();
    }

    [TestMethod]
    public async Task RegisterAgentAsync_ValidRequest_ReturnsAgent()
    {
        // Arrange
        var service = new DeliveryService(_mockRepository!.Object);
        var request = new RegisterAgentRequestDto
        {
            UserId = Guid.NewGuid(),
            FullName = "John Doe",
            Phone = "1234567890",
            VehicleType = VehicleType.BIKE,
            VehicleNumber = "ABC123"
        };

        // Act
        var result = await service.RegisterAgentAsync(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(request.FullName, result.FullName);
        Assert.AreEqual(request.Phone, result.Phone);
        Assert.IsFalse(result.IsVerified);
        Assert.IsFalse(result.IsAvailable);
    }

    [TestMethod]
    public async Task GetAgentByIdAsync_ValidId_ReturnsAgent()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var mockAgent = new QuickBite.DeliveryAgent.Domain.Entities.DeliveryAgent
        {
            Id = agentId,
            UserId = Guid.NewGuid(),
            FullName = "John Doe",
            Phone = "1234567890",
            VehicleType = VehicleType.BIKE,
            VehicleNumber = "ABC123"
        };

        _mockRepository!
            .Setup(r => r.FindByAgentIdAsync(agentId))
            .ReturnsAsync(mockAgent);

        var service = new DeliveryService(_mockRepository.Object);

        // Act
        var result = await service.GetAgentByIdAsync(agentId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(agentId, result.AgentId);
        Assert.AreEqual("John Doe", result.FullName);
    }

    [TestMethod]
    public async Task GetAgentByIdAsync_InvalidId_ReturnsNull()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        _mockRepository!
            .Setup(r => r.FindByAgentIdAsync(agentId))
            .ReturnsAsync((QuickBite.DeliveryAgent.Domain.Entities.DeliveryAgent?)null);

        var service = new DeliveryService(_mockRepository.Object);

        // Act
        var result = await service.GetAgentByIdAsync(agentId);

        // Assert
        Assert.IsNull(result);
    }
}
