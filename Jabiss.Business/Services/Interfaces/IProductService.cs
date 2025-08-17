using Jabiss.Business.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductServiceModel>> GetAllAsync();
        Task<ProductServiceModel?> GetByIdAsync(int id);
        Task<List<ProductServiceModel>> GetByCategoryIdAsync(int categoryId);
        Task<int> AddAsync(ProductServiceModel model);
        Task<bool> UpdateAsync(ProductServiceModel model);
        Task<bool> DeleteAsync(int id);
        Task<int> AddProductImageAsync(int productId, byte[] imageData);
        Task<bool> DeleteProductImageAsync(int imageId);
        Task<int> DeleteImagesForProductAsync(int productId);

    }
}
