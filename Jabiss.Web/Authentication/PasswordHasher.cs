using Jabiss.Business.Services.Interfaces;
using JabissStorage.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Jabiss.Web.Authentication
{
    public class PasswordHasher : IPasswordHasher<User>
    {
        private readonly ISecurityService _securityService;

        public PasswordHasher(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        public string HashPassword(User user, string password)
        {
            return _securityService.Hash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            var providedHash = _securityService.Hash(providedPassword);
            return hashedPassword == providedHash
                ? PasswordVerificationResult.Success
                : PasswordVerificationResult.Failed;
        }
    }

}
