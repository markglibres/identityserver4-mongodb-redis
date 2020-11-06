using System.Collections.Generic;
using System.Reflection;
using Identity.Domain.Abstractions;

namespace Identity.Domain.ValueObjects
{
    public class TenantId : ValueObject
    {
        private readonly string _tenantId;

        private TenantId(string tenantId)
        {
            _tenantId = tenantId;
        }

        public static TenantId Create(string tenantId) => new TenantId(tenantId);
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _tenantId;
        }

        public static TenantId Default => new TenantId(Assembly.GetExecutingAssembly().GetName().Name);
    }
}