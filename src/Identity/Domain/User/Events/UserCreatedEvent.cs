using Identity.Domain.Abstractions;
using Identity.Domain.Attributes;
using Newtonsoft.Json;

namespace Identity.Domain.User.Events
{
    [EventName("Identity.Domain.User.Events","UserCreatedEvent")]
    public class UserCreatedEvent : DomainEvent
    {
        public string Firstname { get; private set;  }
        public string Lastname { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        private UserCreatedEvent() 
        {
            
        }
        
        public UserCreatedEvent(
            IEntityId aggregateId,
            string firstname, string lastname, 
            string email, string password) : base(aggregateId)
        {
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
            Password = password;
        }

    }
}