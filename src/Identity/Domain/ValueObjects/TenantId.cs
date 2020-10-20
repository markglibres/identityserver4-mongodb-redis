using System.Collections.Generic;
using Identity.Domain.Abstractions;

namespace Identity.Domain.ValueObjects
{
    public class TenantId : ValueObject<string>
    {
        private readonly string _tenantId;

        private TenantId(string tenantId)
        {
            _tenantId = tenantId;
        }

        public static TenantId From(string tenantId) => new TenantId(tenantId);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _tenantId;
        }

        public override string Value => _tenantId;
    }
}