using Application.Common.Extensions;
using Application.Flights.DTOs;
using Domain.AbstractServices;
using Domain.Entities;
using Domain.RepositoryAbstraction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Flights.Commands.CreateFlight;
internal class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, FlightDto>
{
    private readonly IRepository _repository;
    private readonly ISessionUser _sessionUser;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CreateFlightCommandHandler> _logger;

    public CreateFlightCommandHandler(IRepository repository, ISessionUser sessionUser, ICacheService cacheService, ILogger<CreateFlightCommandHandler> logger) 
    {
        _repository = repository;
        _sessionUser = sessionUser;
        _cacheService = cacheService;
        _logger = logger;
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
            Created = now,
            CreatedBy = _sessionUser.Username
        };

        await _repository.BeginTransactionAsync(cancellationToken);

        var createdFlight = await _repository.AddAsync<Flight>(flight, cancellationToken);

        await _repository.CommitTransactionAsync(cancellationToken);
        await _cacheService.RemoveAsync("flights:all", cancellationToken);

        _logger.LogInformation(
            "Рейс создан. ID: {FlightId}, Origin: {Origin}, Destination: {Destination}, Пользователь: {Username}, Время: {Time}",
            createdFlight.Id,
            createdFlight.Origin,
            createdFlight.Destination,
            _sessionUser.Username ?? "Система",
            DateTimeOffset.UtcNow);

        return flight.ToDto();
    }
}
