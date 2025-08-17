using Jabiss.Business.Services.Interfaces;
using Jabiss.Business.Services.Models;
using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryServiceModel>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            return categories.Select(p => new CategoryServiceModel
            {
                Id = p.Id,
                Name = p.Name,

            }).ToList();
        }
        public async Task<CategoryServiceModel?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryServiceModel
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task<bool> SaveAsync(CategoryServiceModel model)
        {
            var category = new Category
            {
                Id = model.Id,
                Name = model.Name,
            };

            if (model.Id > 0)
            {
                return await _repository.UpdateAsync(category);
            }
            else
            {
                return await _repository.AddAsync(category);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
