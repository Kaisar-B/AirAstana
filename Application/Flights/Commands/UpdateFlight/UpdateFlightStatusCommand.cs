using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Flights.Commands.UpdateFlight;
public class UpdateFlightStatusCommand: IRequest<Unit>
{
    public int FlightId { get; set; }
    public Status Status { get; set; }
}
