using Common.EventBus;
using DotNetCore.CAP;
using IdentityService.Domain;

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
