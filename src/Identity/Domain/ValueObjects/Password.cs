using System.Collections.Generic;
using Identity.Domain.Abstractions;
using Identity.Domain.Extensions;

namespace Identity.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        private readonly string _password;

        private Password(string password)
        {
            _password = password;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _password;
        }

        public static Password Create(string password)
        {
            var value = new Password(password);
            return value;
        }
    }
}