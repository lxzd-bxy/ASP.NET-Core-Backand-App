using FluentValidation;
using ItLxzdbxy.WebApi.Application.Common.Requests;

namespace ItLxzdbxy.WebApi.Application.Validators;

public class LoginCommandValidator : AbstractValidator<LoginRequest>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}