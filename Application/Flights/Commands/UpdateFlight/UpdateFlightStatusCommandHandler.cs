using Domain.AbstractServices;
using Domain.AppExceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.RepositoryAbstraction;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Flights.Commands.UpdateFlight;
public class UpdateFlightStatusCommandHandler : IRequestHandler<UpdateFlightStatusCommand, Unit>
{
    private readonly ISessionUserService _sessionUser;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateFlightStatusCommandHandler> _logger;
    private readonly TimeProvider _timeProvider;

    public UpdateFlightStatusCommandHandler(ISessionUserService sessionUser, ICacheService cacheService, IUnitOfWork unitOfWork, ILogger<UpdateFlightStatusCommandHandler> logger, TimeProvider timeProvider)
    {
        _sessionUser = sessionUser;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task<Unit> Handle(UpdateFlightStatusCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var flight = await _unitOfWork.Flights.GetByIdAsync(request.FlightId, cancellationToken);

        if (flight == null)
            throw new Exception($"Сущность \"{nameof(Flight)}\" ({request.FlightId}) не найдена.");

        var oldStatus = flight.Status;

        UpdateStatus(_sessionUser, flight, request.Status, _timeProvider);

        await _unitOfWork.Flights.UpdateAsync(flight);

        await _unitOfWork.CommitTransactionAsync(cancellationToken);

        await InvalidateCacheAsync(flight, _cacheService, cancellationToken);

        _logger.LogInformation(
            "Статус рейса изменён. ID: {FlightId}, Старый статус: {OldStatus}, Новый статус: {NewStatus}, Пользователь: {Username}, Время: {Time}",
            flight.Id,
            oldStatus,
            flight.Status,
            _sessionUser.Username ?? "Система",
            DateTimeOffset.UtcNow);

        return Unit.Value;
    }

    private static void UpdateStatus(ISessionUserService sessionUser, Flight flight, Status newStatus, TimeProvider timeProvider)
    {
        if (flight.Status == Status.Cancelled)
            throw new DomainException("Не удается обновить статус отмененного рейса");
        if (flight.Status == newStatus)
            throw new DomainException($"Не удается обновить статус. Текущий статус [{flight.Status}]");
        flight.Status = newStatus;

        flight.LastModifiedBy = sessionUser.Username;
        flight.LastModified = timeProvider.GetUtcNow().ToLocalTime();
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
