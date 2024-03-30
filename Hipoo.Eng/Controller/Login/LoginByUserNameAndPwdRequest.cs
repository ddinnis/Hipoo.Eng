using FluentValidation;

namespace IdentityService.WebAPI.Controller.Login
{
    public record LoginByUserNameAndPwdRequest (string UserName,string Password);
    public class LoginByUserNameAndPwdValidator : AbstractValidator<LoginByUserNameAndPwdRequest> {
        public LoginByUserNameAndPwdValidator()
        {
            RuleFor(e => e.UserName).NotNull().NotEmpty();
            RuleFor(e => e.Password).NotNull().NotEmpty();
        }
    }
}
