using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Authorization;
using IdentityServer.Authorization.Seeders;
using IdentityServer.Users.Authorization.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Users.Authorization.Services
{
    public class UserService<T> : IUserService<T>, ISeedService<T>
        where T : IdentityUser
    {
        private readonly IUserStore<T> _userStore;
        private readonly IPasswordHasher<T> _passwordHasher;
        private readonly IdentityServerConfig _options;

        public UserService(
            IUserStore<T> userStore,
            IPasswordHasher<T> passwordHasher,
            IOptions<IdentityServerConfig> options)
        {
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _options = options.Value;
        }

        public async Task Create(T user, CancellationToken cancellationToken = default)
        {
            var foundUser = await _userStore.FindByNameAsync(user.UserName, cancellationToken);
            if (foundUser != null) return;
            await _userStore.CreateAsync(user, cancellationToken);
        }

        public async Task Update(T user, CancellationToken cancellationToken = default)
        {
            var foundUser = await _userStore.FindByIdAsync(user.Id, cancellationToken);
            if (foundUser == null) return;

            await _userStore.UpdateAsync(user, cancellationToken);
        }

        public async Task<T> GetByUsername(string username, CancellationToken cancellationToken = default)
        {
            var user = await _userStore.FindByNameAsync(username, cancellationToken);
            return user;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            var user = await GetByUsername(username);
            return user != null && user.PasswordHash.Equals(_passwordHasher.HashPassword(user, password));
        }

        public async Task<bool> IsActive(string username)
        {
            var user = await GetByUsername(username);
            return !_options.RequireConfirmedEmail || (user?.EmailConfirmed ?? false);
        }
    }
}
