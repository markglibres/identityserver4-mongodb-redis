using System.Collections.Generic;
using Identity.Domain.Abstractions;
using Identity.Domain.Extensions;

namespace Identity.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        private readonly string _password;

        private Password(string password, bool encrypt = true)
        {
            _password = encrypt ? password.ToSha256() : password;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _password;
        }

        public static Password Create(string password, bool encrypt = true)
        {
            var value = new Password(password, encrypt);
            return value;
        }
    }
}