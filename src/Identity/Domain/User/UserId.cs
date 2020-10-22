using System;
using System.Security.Cryptography;
using Identity.Domain.Abstractions;
using Identity.Domain.ValueObjects;

namespace Identity.Domain.User
{
    public class UserId : AggregateGuid
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
        
    }
}