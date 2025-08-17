using Jabiss.Business.Services.Models;
using JabissStorage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryServiceModel>> GetAllAsync();
        Task<CategoryServiceModel?> GetByIdAsync(int id);
        Task<bool> SaveAsync(CategoryServiceModel category);
        Task<bool> DeleteAsync(int id);
    }
}
