using JabissCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Interfaces
{
    public interface IUserVerificationRepository
    {
        Task<int> CreateOrReplaceAsync(UserVerificationModel model);
        Task<UserVerificationModel?> GetActiveByEmailAsync(string email);
        Task<bool> MarkUsedAsync(int id);
        Task IncrementAttemptAsync(int id);
        Task<int> DeleteExpiredAsync();
    }
}
