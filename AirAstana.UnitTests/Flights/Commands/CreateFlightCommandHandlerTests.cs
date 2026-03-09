using Application.Flights.Commands.CreateFlight;
using Application.Flights.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryAbstraction;
using Domain.AbstractServices;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Application.Flights.DTOs;
using Xunit;

namespace AirAstana.UnitTests.Flights.Commands;

public class CreateFlightCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ISessionUserService> _mockCurrentUserService;
    private readonly Mock<ILogger<CreateFlightCommandHandler>> _mockLogger;
    private readonly CreateFlightCommandHandler _handler;
    private readonly Mock<TimeProvider> _mockTimeProvider;

    public CreateFlightCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCacheService = new Mock<ICacheService>();
        _mockCurrentUserService = new Mock<ISessionUserService>();
        _mockLogger = new Mock<ILogger<CreateFlightCommandHandler>>();
        _mockTimeProvider = new Mock<TimeProvider>();

        _handler = new CreateFlightCommandHandler(
            _mockUnitOfWork.Object,
            _mockCurrentUserService.Object,
            _mockCacheService.Object,
            _mockLogger.Object,
            _mockTimeProvider.Object
            );
    }

    [Fact]
    public async Task Handle_WhenCommandIsValid_ShouldCreateFlight()
    {
        // Arrange
        var command = new CreateFlightCommand
        {
            Origin = "Алматы",
            Destination = "Астана",
            Departure = DateTimeOffset.UtcNow.AddDays(1),
            Arrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            Status = Status.InTime
        };

        var createdFlight = new Flight
        {
            Id = 1,
            Origin = command.Origin,
            Destination = command.Destination,
            Departure = command.Departure,
            Arrival = command.Arrival,
            Status = command.Status
        };

        var flightDto = new FlightDto
        {
            Id = 1,
            Origin = command.Origin,
            Destination = command.Destination,
            Status = "InTime"
        };

        _mockUnitOfWork.Setup(x => x.Flights.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdFlight);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockCurrentUserService.Setup(x => x.Username)
            .Returns("admin");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Origin.Should().Be(command.Origin);
        result.Destination.Should().Be(command.Destination);

        _mockUnitOfWork.Verify(x => x.Flights.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync($"flights:{command.Origin}:{command.Destination}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFlightCreated_ShouldInvalidateCache()
    {
        // Arrange
        var command = new CreateFlightCommand
        {
            Origin = "Алматы",
            Destination = "Астана",
            Departure = DateTimeOffset.UtcNow.AddDays(1),
            Arrival = DateTimeOffset.UtcNow.AddDays(1).AddHours(2),
            Status = Status.InTime
        };

        var createdFlight = new Flight { Id = 1 };
        var flightDto = new FlightDto { Id = 1 };

        // Act
        _mockUnitOfWork.Setup(x => x.Flights.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdFlight);

        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCacheService.Verify(x => x.RemoveAsync($"flights:{command.Origin}:{command.Destination}", It.IsAny<CancellationToken>()), Times.Once);
    }
}