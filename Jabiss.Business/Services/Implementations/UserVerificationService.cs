using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jabiss.Business.Services.Interfaces;
using JabissCommon;
using JabissStorage.Domain.Interfaces;
using System.Security.Cryptography;

namespace Jabiss.Business.Services.Implementations
{
   

    public class UserVerificationService : IUserVerificationService
    {
        private readonly IUserVerificationRepository _repo;
        private readonly IEmailService _email;

        public UserVerificationService(IUserVerificationRepository repo, IEmailService email)
        {
            _repo = repo;
            _email = email;
        }

        public async Task<int> StartAsync(string name, string email, string passwordHash, TimeSpan ttl, CancellationToken ct = default)
        {
            // 6 rəqəmli secure kod
            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

            var model = new UserVerificationModel
            {
                Name = name,
                Email = email,
                PasswordHash = passwordHash,
                Code = code,
                ExpiresAt = DateTime.UtcNow.Add(ttl)
            };

            var id = await _repo.CreateOrReplaceAsync(model);
            await _email.SendAsync(email, "Your Jabiss verification code", $"Your verification code is: {code}");
            return id;
        }

        public async Task<bool> CompleteAsync(string email, string code, Func<UserServiceModel, Task> onVerified, CancellationToken ct = default)
        {
            var rec = await _repo.GetActiveByEmailAsync(email);
            if (rec == null) return false;

            // vaxt keçibsə
            if (rec.ExpiresAt <= DateTime.UtcNow) return false;

            // çox cəhd (məs: 5 limit)
            if (rec.AttemptCount >= 5) return false;

            if (!string.Equals(rec.Code, code, StringComparison.Ordinal))
            {
                await _repo.IncrementAttemptAsync(rec.Id);
                return false;
            }

            // OK → user yaradılır (Role = Customer)
            var userModel = new UserServiceModel
            {
                Name = rec.Name,
                Email = rec.Email,
                PasswordHash = rec.PasswordHash,
                Role = "Customer",
                CreatedAt = DateTime.UtcNow
            };

            await onVerified(userModel);
            await _repo.MarkUsedAsync(rec.Id);
            return true;
        }
    }

}
