using FluentValidation;
using LxzdBxy.Backend.Application.Common.Requests;

namespace LxzdBxy.Backend.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginRequest>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email required").EmailAddress().WithMessage("Incorrect password");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}