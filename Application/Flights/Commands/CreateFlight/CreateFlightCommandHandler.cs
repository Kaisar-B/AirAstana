using Application.Common.Extensions;
using Application.Flights.DTOs;
using Domain.AbstractServices;
using Domain.Entities;
using Domain.RepositoryAbstraction;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Application.Flights.Commands.CreateFlight;
internal class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, FlightDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionUserService _sessionUser;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateFlightCommandHandler> _logger;
    private readonly TimeProvider _timeProvider;

    public CreateFlightCommandHandler(IUnitOfWork unitOfWork, ISessionUserService sessionUser, ICacheService cacheService, ILogger<CreateFlightCommandHandler> logger, TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _sessionUser = sessionUser;
        _cacheService = cacheService;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task<FlightDto> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var flight = new Flight
        {
            Origin = request.Origin,
            Destination = request.Destination,
            Departure = request.Departure,
            Arrival = request.Arrival,
            Status = request.Status,
            Created = _timeProvider.GetUtcNow().ToLocalTime(),
            CreatedBy = _sessionUser.Username
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var createdFlight = await _unitOfWork.Flights.AddAsync(flight, cancellationToken);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        await InvalidateCacheAsync(flight, _cacheService, cancellationToken);

        _logger.LogInformation(
            "Рейс создан. ID: {FlightId}, Origin: {Origin}, Destination: {Destination}, Пользователь: {Username}, Время: {Time}",
            createdFlight.Id,
            createdFlight.Origin,
            createdFlight.Destination,
            _sessionUser.Username ?? "Система",
            _timeProvider.GetUtcNow().ToLocalTime());

        return flight.ToDto();
    }

    private async static Task InvalidateCacheAsync(Flight flight, ICacheService _cacheService, CancellationToken cancellationToken)
    {
        var keysToRemove = new List<string>
        {
            $"flights:{flight.Origin}:{flight.Destination}",
            $"flights:{flight.Origin}:any",
            $"flights:any:{flight.Destination}"
        };

        foreach (var key in keysToRemove)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }
    }
}
