using System;
using System.Security.Cryptography;
using Identity.Domain.Abstractions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.User
{
    public class UserId : AggregateGuidId
    {
        private UserId(TenantId tenantId) : base(tenantId)
        {
            
        }
        private UserId(TenantId tenantId, Guid guid) : base(tenantId, guid)
        {
            
        }
        
        private UserId(string id) : base(id)
        {
            
        }

        public static UserId From(TenantId tenantId) => new UserId(tenantId);
        public static UserId From(TenantId tenantId, Guid guid) => new UserId(tenantId, guid);
        public static UserId From(string id) => new UserId(id);
        
    }
}