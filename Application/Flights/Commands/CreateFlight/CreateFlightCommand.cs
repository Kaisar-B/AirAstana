using Application.Flights.DTOs;
using Domain.Enums;
using MediatR;

namespace Application.Flights.Commands.CreateFlight;
public class CreateFlightCommand : IRequest<FlightDto>
{
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTimeOffset Departure { get; set; }
    public DateTimeOffset Arrival { get; set; }
    public Status Status { get; set; }
}