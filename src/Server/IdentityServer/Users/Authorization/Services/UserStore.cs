using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Authorization;
using IdentityServer.Common.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users.Authorization.Services
{
    public class UserStore<T> : IUserStore<T>, IUserPasswordStore<T>, IUserEmailStore<T>, IUserClaimStore<T>
        where T : IdentityUser
    {
        private readonly IIdentityRepository<T> _identityRepository;
        private readonly IIdentityRepository<ApplicationUserClaim> _claimsRepository;

        public UserStore(
            IIdentityRepository<T> identityRepository,
            IIdentityRepository<ApplicationUserClaim> claimsRepository)
        {
            _identityRepository = identityRepository;
            _claimsRepository = claimsRepository;
        }

        public Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(user.UserName)) return IdentityResult.Failed();
            var result = await FindByNameAsync(user.UserName, CancellationToken.None);
            if (result != null) return IdentityResult.Failed();
            await _identityRepository.Insert(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await FindByIdAsync(user.Id, CancellationToken.None);
            if (result == null) return IdentityResult.Failed();
            await _identityRepository.Delete(u => u.Id == user.Id);
            return IdentityResult.Success;
        }

        public async Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(userId)) throw new RequiredArgumentException(nameof(userId));
            var result = await _identityRepository.SingleOrDefault(u => u.Id == userId);
            return result;
        }

        public async Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(normalizedUserName))
                throw new RequiredArgumentException(nameof(normalizedUserName));
            var result = await _identityRepository.SingleOrDefault(u =>
                u.UserName.ToLowerInvariant() == normalizedUserName.ToLowerInvariant());
            return result;
        }

        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            var normalizedUsername = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();
            return Task.FromResult(normalizedUsername);
        }

        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            if (normalizedName == null) throw new RequiredArgumentException(nameof(normalizedName));
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            if (userName == null) throw new RequiredArgumentException(nameof(userName));
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) return IdentityResult.Failed();
            var result = await FindByIdAsync(user.Id, CancellationToken.None);
            if (result == null) IdentityResult.Failed();
            await _identityRepository.Update(user, u => u.Id == user.Id);
            return IdentityResult.Success;
        }

        public async Task<T> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(normalizedEmail)) throw new RequiredArgumentException(nameof(normalizedEmail));
            var result = await _identityRepository.SingleOrDefault(u => u.Email.ToLowerInvariant() == normalizedEmail.ToLowerInvariant());
            return result;
        }

        public Task<string> GetEmailAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string> GetNormalizedEmailAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            return Task.FromResult(user.NormalizedEmail.ToUpperInvariant());
        }

        public Task SetEmailAsync(T user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            if (email == null) throw new RequiredArgumentException(nameof(email));
            user.Email = email;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(T user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(T user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            if (normalizedEmail == null) throw new RequiredArgumentException(nameof(normalizedEmail));
            user.NormalizedUserName = normalizedEmail;
            return Task.CompletedTask;
        }

        public async Task AddClaimsAsync(T user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));
            var claimsToAdd = claims.ToList();
            if (!claimsToAdd.Any()) return;

            var userClaim = await _claimsRepository.SingleOrDefault(claim => claim.UserId == user.Id);
            var newRecord = userClaim == null;

            if (newRecord) userClaim = new ApplicationUserClaim(user.Id);

            userClaim.AddClaims(claimsToAdd);

            if (newRecord)
                await _claimsRepository.Insert(userClaim);
            else
                await _claimsRepository.Update(userClaim, claim => claim.UserId == user.Id);

        }

        public async Task<IList<Claim>> GetClaimsAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));

            var userClaims = await _claimsRepository.SingleOrDefault(claim => claim.UserId == user.Id);
            return userClaims?.Claims.ToList() ?? new List<Claim>();
        }

        public async Task<IList<T>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (claim == null) throw new RequiredArgumentException(nameof(claim));

            var userClaims = await _claimsRepository.Find(userClaim => userClaim.Claims.Any(c => c.Equals(claim)));
            if (!userClaims.Any()) return default;

            var users = new List<T>();
            foreach (var applicationUserClaim in userClaims)
            {
                var user = await _identityRepository.SingleOrDefault(identityUser =>
                    identityUser.Id == applicationUserClaim.UserId);
                if(user == null) continue;

                users.Add(user);
            }

            return users.ToList();

        }

        public async Task RemoveClaimsAsync(T user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));

            var claimsToRemove = claims.ToList();
            if (!claimsToRemove.Any()) return;

            var userClaims = await _claimsRepository.SingleOrDefault(claim => claim.UserId == user.Id);
            if(userClaims == null || !userClaims.Claims.Any()) return;

            var remainingClaims = userClaims.Claims.Where(existingClaims => claimsToRemove.All(c => !c.Equals(existingClaims)));
            userClaims.Claims = remainingClaims.ToList();

            await _claimsRepository.Update(userClaims, claim => claim.UserId == user.Id);

        }

        public async Task ReplaceClaimAsync(T user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new RequiredArgumentException(nameof(user));

            if(claim == null || newClaim == null) return;

            var userClaims = await _claimsRepository.SingleOrDefault(userClaim => userClaim.UserId == user.Id);
            if(userClaims == null || !userClaims.Claims.Any()) return;

            var remainingClaims = userClaims.Claims.Where(existingClaims => !existingClaims.Equals(claim))
                .ToList();
            remainingClaims.Add(newClaim);

            userClaims.Claims = remainingClaims;

            await _claimsRepository.Update(userClaims, userClaim => userClaim.UserId == user.Id);
        }
    }
}
