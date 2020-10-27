using System;
using System.Collections.Generic;
using Identity.Domain.Abstractions;

namespace Identity.Domain.ValueObjects
{
    public class DateTimeUtc : ValueObject
    {
        private readonly DateTimeOffset _dateTimeOffset;

        private DateTimeUtc(DateTimeOffset dateTimeOffset)
        {
            _dateTimeOffset = dateTimeOffset.UtcDateTime;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _dateTimeOffset.UtcDateTime;
        }
        
        public static DateTimeUtc Now() => new DateTimeUtc(DateTime.UtcNow);
        public static DateTimeUtc From(DateTimeOffset dateTimeOffset) => new DateTimeUtc(dateTimeOffset.UtcDateTime);
        public DateTimeOffset Value => _dateTimeOffset;
    }
}