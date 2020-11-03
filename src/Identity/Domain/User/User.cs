using System;
using System.Collections.Generic;
using Identity.Domain.Abstractions;
using Identity.Domain.User.Events;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.User
{
    public partial class User : Aggregate<UserId>
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
                email.ToString(),
                password.ToString());
            
            Emit(@event);
        }

        protected void Handle(UserCreatedEvent @event)
        {
            Fullname = Fullname.Create(@event.Firstname, @event.Lastname);
            Email = Email.Create(@event.Email);
            Password = Password.Create(@event.Password, false);
        }

        #region Constructors
        public User(UserId id) : base(id)
        {
        }
        protected User(UserId id, IReadOnlyCollection<IDomainEvent> events) : base(id, events)
        {
        }
        #endregion

    }
}