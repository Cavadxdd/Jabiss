using JabissStorage.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JabissStorage.Domain.Interfaces
{
    public interface IProductImageRepository
    {
        Task<IEnumerable<ProductImage>> GetImagesForProductAsync(int productId);
        Task<int> AddImageAsync(int productId, byte[] imageData);
        Task<int> DeleteImagesForProductAsync(int productId);
        Task<bool> DeleteImageAsync(int imageId);

    }
}
