using FluentValidation;

namespace IdentityService.WebAPI.Controller.Admin
{
    public record EditAdminUserRequest(string PhoneNum);
    public class EditAdminUserRequestValidator:AbstractValidator<EditAdminUserRequest>
    {
        public EditAdminUserRequestValidator()
        {
            RuleFor(e => e.PhoneNum).NotNull().NotEmpty();
        }
    }
}
