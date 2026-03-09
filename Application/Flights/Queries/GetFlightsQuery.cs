using Application.Flights.DTOs;
using MediatR;

namespace Application.Flights.Queries;
public class GetFlightsQuery : IRequest<List<FlightDto>>
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
}