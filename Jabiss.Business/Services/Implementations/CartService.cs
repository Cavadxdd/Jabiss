using Jabiss.Business.Services.Interfaces;
using JabissCommon;
using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jabiss.Business.Services.Implementations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using JabissStorage.Domain.Entities;
    using JabissStorage.Domain.Interfaces;
    using JabissCommon;

    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
        }

        public async Task<List<CartItemViewModel>> GetCartItemsAsync(int userId)
        {
            var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return new List<CartItemViewModel>();

            var items = await _cartItemRepository.GetByCartIdAsync(cart.Id);

            return items.Select(i => new CartItemViewModel
            {
                Id = i.Id,
                CartId = i.CartId,
                ProductId = i.ProductId,
                Name = i.Product.Name,
                Price = i.Product.Price,
                Quantity = i.Quantity,
                Stock = i.Product.Stock,
                ImageBase64 = i.Product.ImageDatas != null && i.Product.ImageDatas.Any()
                    ? Convert.ToBase64String(i.Product.ImageDatas.First().ImageData)
                    : null
            }).ToList();
        }

        public async Task<int> AddToCartAsync(int userId, int productId, int quantity)
        {
            // Önce ürünün stok bilgisini al
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found", nameof(productId));
            }

            var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
            {
                var cartId = await _cartRepository.CreateCartAsync(userId);
                cart = new Cart { Id = cartId, UserId = userId };
            }

            var existingCartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id, productId);

            if (existingCartItem != null)
            {
                int newTotalQuantity = existingCartItem.Quantity + quantity;
                // Maksimum miktar: stok miktarı ile 15'in küçük olanı
                int maxAllowedQuantity = Math.Min(product.Stock, 15);
                existingCartItem.Quantity = Math.Min(newTotalQuantity, maxAllowedQuantity);

                await _cartItemRepository.UpdateQuantityAsync(existingCartItem.Id, existingCartItem.Quantity);
                await _cartRepository.UpdateCartUpdatedAtAsync(cart.Id, DateTime.Now);
            }
            else
            {
                // Yeni item ekleme durumunda da stok kontrolü
                int maxAllowedQuantity = Math.Min(product.Stock, 15);
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = Math.Min(quantity, maxAllowedQuantity)
                };

                await _cartItemRepository.AddAsync(cartItem);
                await _cartRepository.UpdateCartUpdatedAtAsync(cart.Id, DateTime.Now);
            }

            var cartCount = await _cartItemRepository.GetCartItemCountAsync(cart.Id);
            return cartCount;
        }
        public async Task<CartItem> GetCartItemByUserAndProductAsync(int userId, int productId)
        {
            var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart == null)
                return null;

            return await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id, productId);
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(0, 0); // productId lazım olsa dəyişdir
            await _cartItemRepository.DeleteAsync(cartItemId);

            if (cartItem != null)
            {
                await _cartRepository.UpdateCartUpdatedAtAsync(cartItem.CartId, DateTime.Now);
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);
            if (cart != null)
            {
                await _cartRepository.DeleteCartAsync(cart.Id);
            }
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(0, 0); // productId lazım olsa dəyişdir
            await _cartItemRepository.UpdateQuantityAsync(cartItemId, quantity);

            if (cartItem != null)
            {
                await _cartRepository.UpdateCartUpdatedAtAsync(cartItem.CartId, DateTime.Now);
            }
        }
    }


}
