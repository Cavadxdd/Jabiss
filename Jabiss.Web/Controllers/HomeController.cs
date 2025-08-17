using Jabiss.Business.Services.Implementations;
using Jabiss.Business.Services.Interfaces;
using Jabiss.Web.Models;
using Jabiss.Web.Models.HomePages;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Jabiss.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;

        public HomeController(IProductService productService, ICategoryService categoryService, ICartService cartService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index(string? sortOption)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int cartCount = 0;

            if (!string.IsNullOrEmpty(userIdClaim))
            {
                int userId = int.Parse(userIdClaim);
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                cartCount = cartItems.Sum(x => x.Quantity);
            }

            ViewBag.CartCount = cartCount;

            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            var sortedProducts = sortOption switch
            {
                "low" => products.OrderBy(p => p.Price).ToList(),
                "high" => products.OrderByDescending(p => p.Price).ToList(),
                _ => products.ToList()
            };

            var productModels = sortedProducts.Select(x => new HomeProductModel
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description ?? "",
                Stock = x.Stock,
                CategoryId = x.CategoryId,
                ImageBase64 = x.Images != null && x.Images.Any()
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(x.Images.First().ImageData)}"
                    : null
            }).ToList();

            var viewModel = new HomeListViewModel
            {
                SortOption = sortOption,
                Products = productModels,
                Categories = categories.Select(c => new HomeCategoryModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ByCategory(int categoryId, string? sortOption)
        {
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            var filtered = products.Where(p => p.CategoryId == categoryId);

            filtered = sortOption switch
            {
                "low" => filtered.OrderBy(p => p.Price),
                "high" => filtered.OrderByDescending(p => p.Price),
                _ => filtered
            };

            var productModels = filtered.Select(x => new HomeProductModel
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description ?? "",
                Stock = x.Stock,
                CategoryId = x.CategoryId,
                ImageBase64 = x.Images != null && x.Images.Any()
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(x.Images.First().ImageData)}"
                    : null
            }).ToList();

            var viewModel = new HomeListViewModel
            {
                SelectedCategoryId = categoryId,
                SortOption = sortOption,
                Products = productModels,
                Categories = categories.Select(c => new HomeCategoryModel
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList()
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var viewModel = new HomeProductDetailModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                Images = product.Images != null
                    ? product.Images
                        .Where(img => img.ImageData != null)
                        .Select(img => $"data:image/jpeg;base64,{Convert.ToBase64String(img.ImageData)}")
                        .ToList()
                    : new List<string>()
            };

            ViewBag.CartCount = 0;
            ViewBag.CartItems = new List<JabissCommon.CartItemViewModel>();

            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = int.Parse(User.FindFirst("Id")!.Value);
                var cartItems = await _cartService.GetCartItemsAsync(userId);
                ViewBag.CartCount = cartItems.Sum(x => x.Quantity);
                ViewBag.CartItems = cartItems;
            }

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? term, int? categoryId)
        {
            var products = await _productService.GetAllAsync();

            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(term))
            {
                products = products
                    .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var productModels = products.Select(x => new HomeProductModel
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description ?? "",
                Stock = x.Stock,
                CategoryId = x.CategoryId,
                ImageBase64 = x.Images != null && x.Images.Any()
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(x.Images.First().ImageData)}"
                    : null
            }).ToList();

            return Json(productModels);
        }

    }
}
