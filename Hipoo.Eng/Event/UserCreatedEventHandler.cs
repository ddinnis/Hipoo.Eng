using DotNetCore.CAP;
using IdentityService.Domain;
using IdentityService.Infrastructure.Service;
using IdentityService.WebAPI.Events;

namespace IdentityService.WebAPI.Event
{
    public class UserCreatedEventHandler: ICapSubscribe,ISubscriberService<ResetPasswordEvent>
    {
        private readonly ISmsSender _smsSender;

        public UserCreatedEventHandler(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        [CapSubscribe("IdentityService.User.Created")]
        public Task HandleSubscriber(ResetPasswordEvent EventData)
        {
            return _smsSender.SendAsync(EventData.PhoneNum, EventData.Password);
        }
    }
}
