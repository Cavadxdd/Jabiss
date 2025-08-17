using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Jabiss.Web.Authentication
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        private readonly IUserRepository _userRepository;

        public UserStore(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var serviceModel = await _userRepository.GetByUsernameAsync(normalizedUserName);

            if (serviceModel == null)
                return null;

            // User entity-ni yarat
            var user = new User
            {
                Id = serviceModel.Id,
                Name = serviceModel.Name,
                Email = serviceModel.Email,
                PasswordHash = serviceModel.PasswordHash,
                Role = serviceModel.Role,
                CreatedAt = serviceModel.CreatedAt
            };

            return user;
        }


        public Task<string?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult<string?>(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Id.ToString());

        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(user.Name);

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken) =>
            Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        // Aşağıdakı metodlar hazırda lazım deyil deyə throw edilir
        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken) =>
            throw new NotImplementedException();
        public void Dispose() { }
    }

}
