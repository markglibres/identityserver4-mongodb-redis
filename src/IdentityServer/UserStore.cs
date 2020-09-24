using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer
{
    public class UserStore<T> : IUserStore<T>, IUserPasswordStore<T>
    where T: IdentityUser
{
	private readonly IRepository<T> _repository;
	public UserStore(IRepository<T> repository)
	{
		_repository = repository;
	}

	public void Dispose()
	{
	}

	public async Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if(string.IsNullOrWhiteSpace(user.UserName)) return IdentityResult.Failed();
		var result = await FindByNameAsync(user.UserName, CancellationToken.None);
		if (result != null) return IdentityResult.Failed();
		await _repository.Insert(user);
		return IdentityResult.Success;
	}

	public async Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
	{
	   cancellationToken.ThrowIfCancellationRequested();
	   var result = await FindByIdAsync(user.Id, CancellationToken.None);
	   if (result == null) return IdentityResult.Failed();
	   await _repository.Delete(u => u.Id == user.Id);
	   return IdentityResult.Success;
	}

	public async Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (string.IsNullOrWhiteSpace(userId)) throw new RequiredArgumentException(nameof(userId));
		var result = await _repository.SingleOrDefault(u => u.Id == userId);
		return result;
	}

	public async Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		if (string.IsNullOrWhiteSpace(normalizedUserName)) throw new RequiredArgumentException(nameof(normalizedUserName));
		var result = await _repository.SingleOrDefault(u =>
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
		await _repository.Update(user, u => u.Id == user.Id);
		return IdentityResult.Success;
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
}
}