using Application.Flights.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Flights.Queries;
public class GetFlightsQuery : IRequest<List<FlightDto>>
{
    public string? Origin { get; set; }
    public string? Destination { get; set; }
}