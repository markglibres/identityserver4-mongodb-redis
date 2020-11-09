using System;
using Identity.Application.Abstractions;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Users.GetUser
{
    public class UserModel : IReadModel
    {
        public string Id { get; set; }
        public string LastCommittedEvent { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}