using DotNetCore.CAP;
using IdentityService.Domain;
using IdentityService.Infrastructure.Service;
using IdentityService.WebAPI.Events;

namespace IdentityService.WebAPI.Event
{
    public class ResetPasswordEventHandler: ICapSubscribe, ISubscriberService<ResetPasswordEvent>
    {
        private readonly ISmsSender _smsSender;

        public ResetPasswordEventHandler( ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }


        [CapSubscribe("IdentityService.User.PasswordReset")]
        public Task HandleSubscriber(ResetPasswordEvent eventData)
        {
            return _smsSender.SendAsync(eventData.PhoneNum, eventData.Password);
        }
    }
}
