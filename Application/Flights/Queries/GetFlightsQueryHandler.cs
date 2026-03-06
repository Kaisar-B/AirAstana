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

namespace Application.Flights.Queries;
public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, List<FlightDto>>
{
    private readonly IRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetFlightsQueryHandler> _logger;
    private const string CacheKey = "flights:all";

    public GetFlightsQueryHandler(IRepository repository, ICacheService cacheService, ILogger<GetFlightsQueryHandler> logger)
    {
        _repository = repository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<List<FlightDto>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        var cachedFlights = await _cacheService.GetAsync<List<FlightDto>>(CacheKey, cancellationToken);
        List<FlightDto> flights;

        if (cachedFlights == null || !cachedFlights.Any())
        {
            _logger.LogInformation("Кэш рейсов пуст, выполняется запрос к базе данных...");

            var flightsFromDb = await _repository.GetAllAsync<Flight>(cancellationToken);

            flights = flightsFromDb.Select(x => x.ToDto()).ToList();

            flights = flights.OrderBy(f => f.Arrival).ToList();

            await _cacheService.SetAsync(CacheKey, flights, TimeSpan.FromHours(1), cancellationToken);
            _logger.LogInformation("Рейсы успешно получены из базы и сохранены в кэш. Всего рейсов: {Count}", flights.Count);
        }
        else
        {
            flights = cachedFlights;
            _logger.LogInformation("Рейсы получены из кэша. Всего рейсов: {Count}", flights.Count);
        }

        if (!string.IsNullOrWhiteSpace(request.Origin))
            flights = flights.Where(f => f.Origin.Contains(request.Origin, StringComparison.OrdinalIgnoreCase)).ToList();
        if (!string.IsNullOrWhiteSpace(request.Destination))
            flights = flights.Where(f => f.Destination.Contains(request.Destination, StringComparison.OrdinalIgnoreCase)).ToList();

        _logger.LogInformation("После применения фильтров по отправлению и назначению осталось рейсов: {Count}", flights.Count);

        return flights;
    }
}