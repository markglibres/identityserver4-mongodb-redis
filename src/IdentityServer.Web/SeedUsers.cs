using System;
using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Web
{
    public class SeedUsers : ISeeder<ApplicationUser>
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public SeedUsers(IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }
        
        public IEnumerable<ApplicationUser> GetSeeds() => new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "dev",
                PasswordHash = _passwordHasher.HashPassword(null, "hardtoguess"),
                EmailConfirmed = true
            }
        };
    }
}