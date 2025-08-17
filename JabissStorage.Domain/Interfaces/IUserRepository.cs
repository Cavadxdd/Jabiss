using JabissCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JabissStorage.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<int> SaveAsync(UserServiceModel model);
        Task<List<UserServiceModel>> GetAllAsync();
        Task<UserServiceModel?> GetByIdAsync(int id);
        Task<UserServiceModel?> GetByUsernameAsync(string username);
        Task<bool> DeleteAsync(int id);
    }
}
