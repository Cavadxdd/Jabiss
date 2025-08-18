using JabissCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Interfaces
{
    public interface IUserVerificationService
    {
        Task<int> StartAsync(string name, string email, string passwordHash, TimeSpan ttl, CancellationToken ct = default);
        Task<bool> CompleteAsync(string email, string code, Func<UserServiceModel, Task> onVerified, CancellationToken ct = default);
    }

}
