using System.Collections.Generic;
using Identity.Domain.Abstractions;

namespace Identity.Domain.ValueObjects
{
    public class Email : ValueObject<string>
    {
        private string _email;

        public Email(string email)
        {
            _email = email;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _email;
        }

        public override string Value => _email;
    }
}