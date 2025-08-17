using Jabiss.Business.Services.Interfaces;
using Jabiss.Business.Services;
using JabissStorage.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Jabiss.Business.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Jabiss.Business.Services.Implementations;
using Jabiss.Web.Models.Categories;

namespace Jabiss.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController:Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categoryViewModel = new CategoryViewModel();

            var categoryDtos = await _categoryService.GetAllAsync();

            categoryViewModel.Categories = categoryDtos.Select(p => new CategoryModel
            {
                     Id = p.Id,
                     Name = p.Name,
            }).ToList();

            return View(categoryViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var categoryModel = new CategoryServiceModel
            {
                Name = viewModel.Name,
            };

            await _categoryService.SaveAsync(categoryModel);

            TempData["SuccessMessage"] = $"Category {viewModel.Name} created successfully";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            var editViewModel = new EditCategoryViewModel
            {
                Id = id,
                Name = category.Name,
            };

            return View(editViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var categoryModel = new CategoryServiceModel
                {
                    Id = viewModel.Id,
                    Name = viewModel.Name,
                };

                var result = await _categoryService.SaveAsync(categoryModel);

                if (result)
                {
                    TempData["SuccessMessage"] = $"Category with name {viewModel.Name} edited successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["ErrorMessage"] = "Edit failed. Please try again.";
                    return View(viewModel);
                }
            }
            catch
            {
                ViewData["ErrorMessage"] = "Something went wrong, please try again";
                return View(viewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);

                var deleted = await _categoryService.DeleteAsync(id);

                if (deleted)
                {
                    TempData["SuccessMessage"] = $"Category with name {category.Name} deleted successfully";
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
    }
}
