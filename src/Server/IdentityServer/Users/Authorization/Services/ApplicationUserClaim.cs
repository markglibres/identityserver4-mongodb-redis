using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityServer.Users.Authorization.Services
{
    public class ApplicationUserClaim
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public IList<Claim> Claims { get; set; }

        public ApplicationUserClaim()
        {

        }
        public ApplicationUserClaim(string userId)
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            Claims = new List<Claim>();
        }

        public void AddClaims(IReadOnlyCollection<Claim> claims)
        {
            claims.ToList().ForEach(claim => Claims.Add(claim));
        }
    }
}
