using IdentityService.Domain.Entities;

namespace IdentityService.WebAPI.Controller.Admin
{
    //public class UserDTO(Guid Id, string UserName, string PhoneNumber, DateTime CreationTime)
    //{
    //    public static UserDTO Create(User user) {
    //        return new UserDTO(user.Id, user.UserName, user.PhoneNumber, user.CreationTime);
    //    }
    //}

    public record UserDTO
    {
        public Guid Id { get; init; }
        public string UserName { get; init; }
        public string PhoneNumber { get; init; }
        public DateTime CreationTime { get; init; }

        public UserDTO(Guid id, string userName, string phoneNumber, DateTime creationTime) =>
            (Id, UserName, PhoneNumber, CreationTime) = (id, userName, phoneNumber, creationTime);

        public static UserDTO Create(User user) =>
            new(user.Id, user.UserName, user.PhoneNumber, user.CreationTime);
    }

}
