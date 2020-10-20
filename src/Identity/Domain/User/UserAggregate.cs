using System;
using System.Collections.Generic;
using Identity.Domain.Abstractions;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.User
{
    public partial class UserAggregate : Aggregate<UserId>
    {
        public Fullname Fullname { get; private set; }
        public Email Email { get; private set; }
        public Password Password { get; private set; }

        public void Create(Fullname fullname, Email email, Password password)
        {
            var @event = new UserCreatedEvent(
                Id,
                fullname.Firstname,
                fullname.Lastname,
                email.Value,
                password.Value);
            
            Emit(@event);
        }

        public void Handle(UserCreatedEvent @event)
        {
            Fullname = new Fullname(@event.Firstname, @event.Lastname);
            Email = new Email(@event.Email);
            Password = new Password(@event.Password, false);
        }

        #region Constructors
        public UserAggregate(UserId id) : base(id)
        {
        }
        public UserAggregate(UserId id, IReadOnlyCollection<IDomainEvent> events) : base(id, events)
        {
        }
        #endregion

    }
}