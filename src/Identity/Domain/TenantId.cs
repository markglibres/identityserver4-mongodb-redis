using System.Collections.Generic;

namespace Identity.Domain
{
    public class TenantId : ValueObject<string>
    {
        private readonly string _tenantId;

        public TenantId(string tenantId)
        {
            _tenantId = tenantId;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _tenantId;
        }

        public override string Value => _tenantId;
    }
}