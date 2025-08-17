using Microsoft.AspNetCore.Mvc;
using Jabiss.Business.Services.Interfaces;
using Jabiss.Web.Models;
using System;
using JabissCommon;
using Microsoft.AspNetCore.Authorization;
using JabissStorage.Domain.Interfaces;

namespace Jabiss.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService, ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            _cartService = cartService;
            _productService = productService;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst("Id")!.Value); 
            var cartItems = await _cartService.GetCartItemsAsync(userId);

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Json(new { success = false, message = "User not logged in" });
            }

            int userId = int.Parse(userIdClaim);

            try
            {
                var existingItem = await _cartService.GetCartItemByUserAndProductAsync(userId, productId);
                int existingQty = existingItem?.Quantity ?? 0;

                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                int remainingLimit = Math.Min(15 - existingQty, product.Stock);
                if (remainingLimit <= 0)
                {
                    return Json(new { success = false, message = "Maximum limit reached for this product" });
                }

                if (quantity > remainingLimit)
                {
                    quantity = remainingLimit; 
                }

                var cartCount = await _cartService.AddToCartAsync(userId, productId, quantity);
                return Json(new { success = true, cartCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            await _cartService.RemoveFromCartAsync(cartItemId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst("Id")!.Value);
            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            await _cartService.UpdateQuantityAsync(cartItemId, quantity);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetAddToCartOptions(int productId)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Json(new { success = false, canAddToCart = false, reason = "Not authenticated" });

                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

                var product = await _productService.GetByIdAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, canAddToCart = false, reason = "Product not found" });
                }

                var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId);
                var currentQuantity = 0;

                if (cart != null)
                {
                    var existingCartItem = await _cartItemRepository.GetByCartIdAndProductIdAsync(cart.Id, productId);
                    currentQuantity = existingCartItem?.Quantity ?? 0;
                }

                var remainingLimit = Math.Max(0, 15 - currentQuantity);
                var remainingStock = Math.Max(0, product.Stock - currentQuantity);
                var maxQuantity = Math.Min(remainingLimit, remainingStock);

                string reason = null;
                if (maxQuantity <= 0)
                {
                    if (currentQuantity >= product.Stock)
                    {
                        reason = $"Product stock is exhausted. Maximum {product.Stock} items available and all are already added to cart.";
                    }
                    else if (currentQuantity >= 15)
                    {
                        reason = $"You can add maximum 15 items to cart. Currently {currentQuantity} items are added.";
                    }
                }

                return Json(new
                {
                    success = true,
                    canAddToCart = maxQuantity > 0,
                    maxQuantity = maxQuantity,
                    currentInCart = currentQuantity,
                    productStock = product.Stock,
                    reason = reason
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, canAddToCart = false, reason = "An error occurred" });
            }
        }
    }
}
