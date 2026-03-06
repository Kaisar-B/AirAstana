using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Flights.Commands.UpdateFlight;
public class UpdateFlightStatusCommandValidator : AbstractValidator<UpdateFlightStatusCommand>
{
    public UpdateFlightStatusCommandValidator()
    {
        RuleFor(x => x.FlightId)
            .GreaterThan(0)
            .WithMessage("Идентификатор рейса должен быть положительным числом");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Выбран недопустимый статус рейса");
    }
}
