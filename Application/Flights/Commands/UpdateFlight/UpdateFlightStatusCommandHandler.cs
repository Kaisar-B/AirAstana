using Domain.AbstractServices;
using Domain.AppExceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryAbstraction;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Application.Flights.Commands.UpdateFlight;
public class UpdateFlightStatusCommandHandler : IRequestHandler<UpdateFlightStatusCommand, Unit>
{
    private readonly ISessionUser _sessionUser;
    private readonly ICacheService _cacheService;
    private readonly IRepository _repository;
    private readonly ILogger<UpdateFlightStatusCommandHandler> _logger;

    public UpdateFlightStatusCommandHandler(ISessionUser sessionUser, ICacheService cacheService, IRepository repository, ILogger<UpdateFlightStatusCommandHandler> logger)
    {
        _sessionUser = sessionUser;
        _cacheService = cacheService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateFlightStatusCommand request, CancellationToken cancellationToken)
    {
        await _repository.BeginTransactionAsync(cancellationToken);

        var flight = await _repository.GetByIdAsync<Flight>(request.FlightId, cancellationToken);

        if (flight == null)
            throw new Exception($"Сущность \"{nameof(Domain.Entities.Flight)}\" ({request.FlightId}) не найдена.");

        var oldStatus = flight.Status;
        
        UpdateStatus( _sessionUser, flight, request.Status);

        _repository.Update<Flight>(flight);
        
        await _repository.CommitTransactionAsync(cancellationToken);

        await _cacheService.RemoveAsync("flights:all", cancellationToken);

        _logger.LogInformation(
            "Статус рейса изменён. ID: {FlightId}, Старый статус: {OldStatus}, Новый статус: {NewStatus}, Пользователь: {Username}, Время: {Time}",
            flight.Id,
            oldStatus,
            flight.Status,
            _sessionUser.Username ?? "Система",
            DateTimeOffset.UtcNow);

        return Unit.Value;
    }

    private static void UpdateStatus(ISessionUser sessionUser, Flight flight ,Status newStatus)
    {
        if (flight.Status == Status.Cancelled)
            throw new DomainException("Не удается обновить статус отмененного рейса");
        if (flight.Status == newStatus)
            throw new DomainException($"Не удается обновить статус. Текущий статус [{flight.Status}]");
        flight.Status = newStatus;

        var now = DateTimeOffset.UtcNow;
        flight.LastModifiedBy = sessionUser.Username;
        flight.LastModified = now;
    }
}
