using Application.Common.Extensions;
using Application.Flights.DTOs;
using Domain.AbstractServices;
using Domain.RepositoryAbstraction;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Flights.Queries;
public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, List<FlightDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetFlightsQueryHandler> _logger;

    public GetFlightsQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<GetFlightsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<List<FlightDto>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"flights:{request.Origin ?? "any"}:{request.Destination ?? "any"}";

        var cachedFlights = await _cacheService.GetAsync<List<FlightDto>>(cacheKey, cancellationToken);
        List<FlightDto> flights;

        if (cachedFlights == null || !cachedFlights.Any())
        {
            _logger.LogInformation("Кэш рейсов пуст, выполняется запрос к базе данных...");

            var flightsFromDb = await _unitOfWork.Flights.WhereAsync(request.Origin, request.Destination, true);

            flights = flightsFromDb.Select(x => x.ToDto()).ToList();

            await _cacheService.SetAsync(cacheKey, flights, TimeSpan.FromHours(1), cancellationToken);
            _logger.LogInformation("Рейсы успешно получены из базы и сохранены в кэш. Всего рейсов: {Count}", flights.Count);
        }
        else
        {
            flights = cachedFlights;
            _logger.LogInformation("Рейсы получены из кэша. Всего рейсов: {Count}", flights.Count);
        }

        _logger.LogInformation("После применения фильтров по отправлению и назначению осталось рейсов: {Count}", flights.Count);

        return flights;
    }
}