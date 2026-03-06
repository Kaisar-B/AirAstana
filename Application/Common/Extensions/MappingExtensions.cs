using Application.Flights.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Auth.Models.DTOs;
using Domain.Enums;

namespace Application.Common.Extensions;

public static class MappingExtensions
{
    public static FlightDto ToDto(this Flight flight)
    {
        return new FlightDto
        {
            Id = flight.Id,
            Origin = flight.Origin,
            Destination = flight.Destination,
            Departure = flight.Departure,
            Arrival = flight.Arrival,
            Status = flight.Status.ToString()
        };
    }

    public static string ToDto(this Status status)
    {
        return status.ToString();
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Role = user.Role.Code
        };
    }

    public static RoleDto ToDto(this Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Code = role.Code,
        };
    }
}