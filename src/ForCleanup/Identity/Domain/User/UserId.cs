using System;
using System.Security.Cryptography;
using Identity.Domain.Abstractions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.User
{
    public class UserId : AggregateGuid
    {
        private UserId(TenantId tenantId = null) : base(tenantId)
        {
            
        }
        private UserId(Guid guid, TenantId tenantId = null) : base(guid, tenantId)
        {
            
        }
        
        private UserId(string id) : base(id)
        {
            
        }

        public static UserId Create(string tenantId = null) => Create<UserId>(tenantId);
        public static UserId Create(Guid id, string tenantId = null) => Create<UserId>(id, tenantId);

    }
}