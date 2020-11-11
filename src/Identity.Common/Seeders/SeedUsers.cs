using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Identity.Common.Seeders
{
    public class SeedUsers<T> : ISeeder<T>
        where T: IdentityUser
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
            instance.UserName = "dev";
            instance.PasswordHash = _passwordHasher.HashPassword(null, "hardtoguess");
            instance.EmailConfirmed = true;
            
            return new List<T> { instance };
        }
    }
}