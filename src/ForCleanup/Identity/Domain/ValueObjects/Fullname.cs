using System.Collections.Generic;
using Identity.Domain.Abstractions;

namespace Identity.Domain.ValueObjects
{
    public class Fullname : ValueObject
    {
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }

        private Fullname(string firstname, string lastname)
        {
            Firstname = firstname;
            Lastname = lastname;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Firstname;
            yield return Lastname;
        }

        public override string ToString() => $"{Firstname} {Lastname}";

        public static Fullname Create(string firstname, string lastname)
        {
            var value = new Fullname(firstname, lastname);
            return value;
        }
    }
}