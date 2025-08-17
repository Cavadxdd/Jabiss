using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jabiss.Business.Services.Interfaces;
using JabissCommon;
using JabissStorage.Domain.Interfaces;


namespace Jabiss.Business.Services.Implementations
{
    public class UserService:IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> SaveAsync(UserServiceModel model)
        {
            return await _userRepository.SaveAsync(model);
        }

        public async Task<List<UserServiceModel>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserServiceModel?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<UserServiceModel?> GetByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }
    }
}
