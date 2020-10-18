using System.Collections.Generic;

namespace Identity.Domain
{
    public class Password : ValueObject<string>
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

        public override string Value => _password;
    }
}