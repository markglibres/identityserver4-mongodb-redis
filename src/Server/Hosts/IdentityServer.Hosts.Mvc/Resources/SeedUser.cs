using System;
using System.Collections.Generic;
using IdentityServer.Authorization.Seeders;
using IdentityServer.Users.Authorization.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Hosts.Mvc.Resources
{
    public class SeedUser : ISeeder<ApplicationUser>
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public SeedUser(IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }
        public IEnumerable<ApplicationUser> GetSeeds()
        {
            var instance = new ApplicationUser{};
            instance.Id = Guid.NewGuid().ToString();
            instance.UserName = "dev@example.com";
            instance.PasswordHash = _passwordHasher.HashPassword(null, "hardtoguess");
            instance.EmailConfirmed = true;
            instance.Email = instance.UserName;

            return new List<ApplicationUser> { instance };
        }
    }
}
