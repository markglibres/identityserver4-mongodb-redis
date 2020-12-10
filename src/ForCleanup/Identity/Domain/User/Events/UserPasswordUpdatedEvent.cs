using Identity.Domain.Abstractions;
using Identity.Domain.Attributes;

namespace Identity.Domain.User.Events
{
    [EventName("Identity.Domain.User.Events","UserPasswordUpdatedEvent")]
    public class UserPasswordUpdatedEvent : DomainEvent
    {
        public string HashedPassword { get; private set; }
        private UserPasswordUpdatedEvent(){}

        public UserPasswordUpdatedEvent(UserId userId, string hashedPassword) 
            : base(userId.Id.ToString(), userId.TenantId.ToString())
        {
            HashedPassword = hashedPassword;
        }
    }
}