using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Authorization.Seeders
{
    public class SeedUsers<T> : ISeeder<T>
        where T : IdentityUser
    {
        private readonly IPasswordHasher<T> _passwordHasher;

        public SeedUsers(IPasswordHasher<T> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public IEnumerable<T> GetSeeds()
        {
            var instance = Activator.CreateInstance<T>();
            instance.Id = Guid.NewGuid().ToString();
            instance.UserName = "dev@example.com";
            instance.PasswordHash = _passwordHasher.HashPassword(null, "hardtoguess");
            instance.EmailConfirmed = true;
            instance.Email = instance.UserName;

            return new List<T> {instance};
        }
    }
}