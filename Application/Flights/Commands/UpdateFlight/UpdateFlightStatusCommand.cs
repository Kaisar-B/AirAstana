using Domain.Enums;
using MediatR;

namespace Application.Flights.Commands.UpdateFlight;
public class UpdateFlightStatusCommand : IRequest<Unit>
{
    public int FlightId { get; set; }
    public Status Status { get; set; }
}
