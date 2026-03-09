using FluentValidation;

namespace Application.Auth.Commands;
internal class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Имя пользователя обязательно к указанию")
            .MaximumLength(15).WithMessage("Имя пользователя не должно превышать 15 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(3).WithMessage("Пароль должен содержать минимум 3 символов");
    }
}
