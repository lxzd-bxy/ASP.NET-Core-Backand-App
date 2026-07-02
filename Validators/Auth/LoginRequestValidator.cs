using FluentValidation;
using ItLxzdbxy.WebApi.DTOs.Auth;

namespace ItLxzdbxy.WebApi.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}