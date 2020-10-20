using System.Collections.Generic;
using Identity.Domain.Abstractions;

namespace Identity.Domain.ValueObjects
{
    public class Fullname : ValueObject<string>
    {
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }

        public Fullname(string firstname, string lastname)
        {
            Firstname = firstname;
            Lastname = lastname;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Firstname;
            yield return Lastname;
        }

        public override string Value => $"{Firstname} {Lastname}";
    }
}