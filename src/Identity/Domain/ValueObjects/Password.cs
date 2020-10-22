using System.Collections.Generic;
using Identity.Domain.Abstractions;
using Identity.Domain.Extensions;

namespace Identity.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        private readonly string _password;

        public Password(string password, bool encrypt = true)
        {
            _password = encrypt ? password.ToSha256() : password;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _password;
        }
    }
}