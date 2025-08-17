using Jabiss.Business.Services.Interfaces;
using Jabiss.Web.Models.Products;
using Jabiss.Business.Services;
using JabissStorage.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Jabiss.Business.Services.Models;
using Microsoft.AspNetCore.Authorization;

namespace Jabiss.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
            private readonly IProductService _productService;
            private readonly ICategoryService _categoryService;

            public ProductsController (IProductService productService, ICategoryService categoryService)
            {
                _productService = productService;
                _categoryService = categoryService;
            }

            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var productViewModel = new ProductViewModel();

                var productDtos = await _productService.GetAllAsync();

                productViewModel.Products = productDtos.Select(p => new ProductModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    CategoryId = p.CategoryId,
                    Images = p.Images.Select(i => new ProductImageModel
                    {
                        Id = i.Id,
                        ImageData = i.ImageData
                    }).ToList()
                }).ToList();

                return View(productViewModel);
            }

            [HttpGet]
            public IActionResult Create()
            {
                ViewBag.Categories = _categoryService.GetAllAsync().Result;
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> Create(CreateProductViewModel viewModel)
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = _categoryService.GetAllAsync().Result;
                    return View(viewModel);
                }

                var productModel = new ProductServiceModel
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Stock = viewModel.Stock,
                    CategoryId = viewModel.CategoryId,
                    Images = viewModel.Images?.Select(i => new ProductImageServiceModel
                    {
                        ImageData = i.ImageData
                    }).ToList() ?? new List<ProductImageServiceModel>()
                };

                await _productService.AddAsync(productModel);

                TempData["SuccessMessage"] = $"Product {viewModel.Name} created successfully";

                return RedirectToAction("Index");
            }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            var editViewModel = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                Images = product.Images?.Select(i => new ProductImageViewModel
                {
                    Id = i.Id,
                    ImageBase64 = i.ImageData != null && i.ImageData.Length > 0
                        ? "data:image/jpeg;base64," + Convert.ToBase64String(i.ImageData)
                        : string.Empty
                }).ToList() ?? new List<ProductImageViewModel>()
            };

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(viewModel);
            }

            try
            {
                var existing = await _productService.GetByIdAsync(viewModel.Id);
                if (existing == null) return NotFound();

                static byte[]? ConvertBase64ToBytes(string? data)
                {
                    if (string.IsNullOrWhiteSpace(data)) return null;
                    var s = data.Trim();
                    var commaIdx = s.IndexOf(',');
                    if (commaIdx >= 0) s = s.Substring(commaIdx + 1);
                    return Convert.FromBase64String(s);
                }

                var finalImages = new List<ProductImageServiceModel>();
                if (viewModel.Images != null)
                {
                    foreach (var img in viewModel.Images)
                    {
                        var bytes = ConvertBase64ToBytes(img.ImageBase64);
                        if (bytes != null && bytes.Length > 0)
                        {
                            finalImages.Add(new ProductImageServiceModel
                            {
                                Id = img.Id,
                                ProductId = viewModel.Id,
                                ImageData = bytes
                            });
                        }
                        else
                        {
                        }
                    }
                }

                var productModel = new ProductServiceModel
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    Stock = viewModel.Stock,
                    CategoryId = viewModel.CategoryId,
                    Images = finalImages
                };

                var result = await _productService.UpdateAsync(productModel);
                if (result)
                {
                    TempData["SuccessMessage"] = $"Product '{viewModel.Name}' updated successfully.";
                    return RedirectToAction("Index");
                }

                ViewData["ErrorMessage"] = "Edit failed. Please try again.";
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(viewModel);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("", "One of the images is not a valid base64 string.");
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(viewModel);
            }
            catch (Exception)
            {
                ViewData["ErrorMessage"] = "Something went wrong, please try again.";
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(viewModel);
            }
        }



        [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                try
                {
                    var product = await _productService.GetByIdAsync(id);
                    if (product == null)
                    {
                        TempData["ErrorMessage"] = "Product not found.";
                        return RedirectToAction("Index");
                    }

                    var deleted = await _productService.DeleteAsync(id);

                    if (deleted)
                    {
                        TempData["SuccessMessage"] = $"Product with name {product.Name} deleted successfully";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Delete failed. Please try again.";
                    }

                    return RedirectToAction("Index");
                }
                catch
                {
                    TempData["ErrorMessage"] = "Something went wrong, please try again";
                    return RedirectToAction("Index");
                }
            }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int productId, int imageId)
        {
            var result = await _productService.DeleteProductImageAsync(imageId);
            if (result)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
