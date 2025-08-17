using Jabiss.Business.Services.Interfaces;
using Jabiss.Business.Services.Models;
using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;

        public ProductService(IProductRepository productRepository, IProductImageRepository productImageRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _productImageRepository = productImageRepository ?? throw new ArgumentNullException(nameof(productImageRepository));
        }

        public async Task<List<ProductServiceModel>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            var productModels = new List<ProductServiceModel>();

            foreach (var product in products)
            {
                productModels.Add(new ProductServiceModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId,
                    Images = product.ImageDatas.Select(img => new ProductImageServiceModel
                    {
                        Id = img.Id,
                        ProductId = img.ProductId,
                        ImageData = img.ImageData
                    }).ToList()
                });
            }

            return productModels;
        }

        public async Task<ProductServiceModel?> GetByIdAsync(int id)
        {

            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return null;
            }

            return new ProductServiceModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                Images = product.ImageDatas.Select(img => new ProductImageServiceModel
                {
                    Id = img.Id,
                    ProductId = img.ProductId,
                    ImageData = img.ImageData
                }).ToList()
            };
        }

        public async Task<List<ProductServiceModel>> GetByCategoryIdAsync(int categoryId)
        {

            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            var productModels = new List<ProductServiceModel>();

            foreach (var product in products)
            {
                productModels.Add(new ProductServiceModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    CategoryId = product.CategoryId,
                    Images = product.ImageDatas.Select(img => new ProductImageServiceModel
                    {
                        Id = img.Id,
                        ProductId = img.ProductId,
                        ImageData = img.ImageData
                    }).ToList()
                });
            }

            return productModels;
        }

        public async Task<int> AddAsync(ProductServiceModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                ImageDatas = model.Images?.Select(img => new ProductImage
                {
                    ProductId = 0, // ID will be set after product insertion
                    ImageData = img.ImageData
                }).ToList() ?? new List<ProductImage>()
            };

            return await _productRepository.AddAsync(product);
        }

        public async Task<bool> UpdateAsync(ProductServiceModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Stock = model.Stock,
                CategoryId = model.CategoryId,
                ImageDatas = model.Images?.Select(img => new ProductImage
                {
                    Id = img.Id,
                    ProductId = model.Id,
                    ImageData = img.ImageData
                }).ToList() ?? new List<ProductImage>()
            };

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {

            return await _productRepository.DeleteAsync(id);
        }

        public async Task<int> AddProductImageAsync(int productId, byte[] imageData)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Ürün ID sıfırdan büyük olmalıdır.", nameof(productId));
            }

            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("Resim verisi boş olamaz.", nameof(imageData));
            }

            return await _productImageRepository.AddImageAsync(productId, imageData);
        }

        public async Task<bool> DeleteProductImageAsync(int imageId)
        {
            if (imageId <= 0)
            {
                throw new ArgumentException("Resim ID sıfırdan büyük olmalıdır.", nameof(imageId));
            }

            return await _productImageRepository.DeleteImageAsync(imageId);
        }

        public async Task<int> DeleteImagesForProductAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Ürün ID sıfırdan büyük olmalıdır.", nameof(productId));
            }

            return await _productImageRepository.DeleteImagesForProductAsync(productId);
        }
    

}
}
