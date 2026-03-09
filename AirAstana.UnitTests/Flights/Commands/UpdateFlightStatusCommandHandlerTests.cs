using Application.Flights.Commands.UpdateFlight;
using Domain.AbstractServices;
using Domain.Entities;
using Domain.RepositoryAbstraction;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirAstana.UnitTests.Flights.Commands;
public class UpdateFlightStatusCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<ISessionUserService> _mockSessionUserService;
    private readonly Mock<ILogger<UpdateFlightStatusCommandHandler>> _mockLogger;
    private readonly UpdateFlightStatusCommandHandler _handler;
    private readonly Mock<TimeProvider> _mockTimeProvider;

    public UpdateFlightStatusCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCacheService = new Mock<ICacheService>();
        _mockSessionUserService = new Mock<ISessionUserService>();
        _mockLogger = new Mock<ILogger<UpdateFlightStatusCommandHandler>>();
        _mockTimeProvider = new Mock<TimeProvider>();

        _handler = new UpdateFlightStatusCommandHandler(
            _mockSessionUserService.Object,
            _mockCacheService.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object,
            _mockTimeProvider.Object);
    }

    [Fact]
    public async Task Handle_WhenFlightExists_ShouldUpdateStatus()
    {
        // Arrange 

        var command = new UpdateFlightStatusCommand
        {
            FlightId = 1,
            Status = Status.Delayed
        };

        var flight = new Flight
        {
            Id = 1,
            Status = Status.InTime
        };

        _mockUnitOfWork.Setup(x => x.Flights.GetByIdAsync(command.FlightId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flight);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        flight.Status.Should().Be(Status.Delayed);
        _mockUnitOfWork.Verify(x => x.Flights.UpdateAsync(flight, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(x => x.RemoveAsync($"flights:{flight.Origin}:{flight.Destination}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenFlightDoesNotExist_ShouldThrowNotFoundException()
    {
        // Arrange
        var command = new UpdateFlightStatusCommand
        {
            FlightId = 999,
            Status = Status.Delayed
        };

        _mockUnitOfWork.Setup(x => x.Flights.GetByIdAsync(command.FlightId, It.IsAny<CancellationToken>())).ReturnsAsync((Flight?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage($"Сущность \"{nameof(Flight)}\" ({command.FlightId}) не найдена.");
    }
}