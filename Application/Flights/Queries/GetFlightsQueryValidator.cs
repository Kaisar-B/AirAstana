using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Flights.Queries;
public class GetFlightsQueryValidator : AbstractValidator<GetFlightsQuery>
{
    public GetFlightsQueryValidator()
    {
        RuleFor(x => x.Origin)
            .MaximumLength(256)
            .WithMessage("Длина пункта отправления не может превышать 256 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Origin));

        RuleFor(x => x.Destination)
            .MaximumLength(256)
            .WithMessage("Длина пункта назначения не может превышать 256 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Destination));
    }
}
