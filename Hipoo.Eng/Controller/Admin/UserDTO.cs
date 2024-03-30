using IdentityService.Domain.Entities;

namespace IdentityService.WebAPI.Controller.Admin
{
    public class UserDTO(Guid Id, string UserName, string PhoneNumber, DateTime CreationTime)
    {
        public static UserDTO Create(User user) {
            return new UserDTO(user.Id, user.UserName, user.PhoneNumber, user.CreationTime);
        }
    }
}
