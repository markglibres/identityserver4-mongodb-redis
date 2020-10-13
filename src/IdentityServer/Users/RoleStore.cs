using System.Threading;
using System.Threading.Tasks;
using IdentityServer.Repositories.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Users
{
    public class RoleStore<T> : IRoleStore<T>
        where T : IdentityRole
    {
        private readonly IIdentityRepository<T> _identityRepository;

        public RoleStore(IIdentityRepository<T> identityRepository)
        {
            _identityRepository = identityRepository;
        }

        public void Dispose()
        {
        }

        public async Task<IdentityResult> CreateAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await FindByNameAsync(role.Name, CancellationToken.None);
            if (result != null) return IdentityResult.Failed();
            await _identityRepository.Insert(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var result = await FindByIdAsync(role.Id, CancellationToken.None);
            if (result == null) return IdentityResult.Failed();
            await _identityRepository.Delete(r => r.Id == role.Id);
            return IdentityResult.Success;
        }

        public async Task<T> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(roleId)) throw new RequiredArgumentException(nameof(roleId));
            var result = await _identityRepository.SingleOrDefault(r => r.Id == roleId);
            return result;
        }

        public async Task<T> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(normalizedRoleName))
                throw new RequiredArgumentException(nameof(normalizedRoleName));
            var result = await _identityRepository.SingleOrDefault(r => r.NormalizedName == normalizedRoleName);
            return result;
        }

        public Task<string> GetNormalizedRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new RequiredArgumentException(nameof(role));
            return Task.FromResult(role.NormalizedName ?? role.Name.ToUpperInvariant());
        }

        public Task<string> GetRoleIdAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new RequiredArgumentException(nameof(role));
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new RequiredArgumentException(nameof(role));
            return Task.FromResult(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(T role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new RequiredArgumentException(nameof(role));
            if (string.IsNullOrWhiteSpace(normalizedName)) throw new RequiredArgumentException(nameof(normalizedName));
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(T role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new RequiredArgumentException(nameof(role));
            if (string.IsNullOrWhiteSpace(roleName)) throw new RequiredArgumentException(nameof(roleName));
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) return IdentityResult.Failed();
            var result = await FindByIdAsync(role.Id, CancellationToken.None);
            if (result == null) IdentityResult.Failed();
            await _identityRepository.Update(role, r => r.Id == role.Id);
            return IdentityResult.Success;
        }
    }
}