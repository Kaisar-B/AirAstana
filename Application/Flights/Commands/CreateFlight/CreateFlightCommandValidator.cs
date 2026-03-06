using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Flights.Commands.CreateFlight;
public class CreateFlightCommandValidator : AbstractValidator<CreateFlightCommand>
{
    public CreateFlightCommandValidator()
    {
        RuleFor(x => x.Origin)
            .NotEmpty().WithMessage("Укажите город отправления")
            .MaximumLength(256).WithMessage("Город отправления не должен превышать 256 символов");

        RuleFor(x => x.Destination)
            .NotEmpty().WithMessage("Укажите город назначения")
            .MaximumLength(256).WithMessage("Город назначения не должен превышать 256 символов")
            .NotEqual(x => x.Origin).WithMessage("Город назначения не может совпадать с городом отправления");

        RuleFor(x => x.Departure)
            .NotEmpty().WithMessage("Необходимо указать время вылета")
            .GreaterThan(DateTimeOffset.UtcNow).WithMessage("Время вылета должно быть позже текущего момента");

        RuleFor(x => x.Arrival)
            .NotEmpty().WithMessage("Необходимо указать время прилёта")
            .GreaterThan(x => x.Departure).WithMessage("Время прилёта должно быть после времени вылета");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Выбран некорректный статус рейса")
            .NotEqual(Status.Cancelled).WithMessage("Нельзя создавать рейс со статусом 'Отменён'");
    }
}