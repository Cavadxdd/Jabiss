using Jabiss.Business.Services;
using Jabiss.Business.Services.Implementations;
using Jabiss.Business.Services.Interfaces;
using Jabiss.Web.Models;
using Jabiss.Web.Models.Account;
using Jabiss.Web.Models.HomePages;
using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using JabissCommon;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace Jabiss.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly ISecurityService _securityService;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IUserService userService, ISecurityService securityService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userService = userService;
            _securityService = securityService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _signInManager.UserManager.FindByNameAsync(model.Name);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(model);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Password is incorrect");
                return View(model);
            }

                var claims = new List<Claim>
                  {
                          new Claim("Id", user.Id.ToString()),  // Burada Id claim'i ekleniyor
                          new Claim(ClaimTypes.Name, user.Name),
                          new Claim(ClaimTypes.Role, user.Role),
                          new Claim(ClaimTypes.Email, user.Email),
                          new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                  };
                 
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            if (model.LoginAsAdmin)
            {
                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Products");
                else
                {
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError("", "This user is not admin");
                    return View(model);
                }
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // POST: Account/SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Eyni email varsa qəbul etmə
            var existingUser = await _userService.GetByUsernameAsync(model.Name);
            if (existingUser != null)
            {
                ModelState.AddModelError("Name", "This username is already taken.");
                return View(model);
            }

            // Şifrəni hash-lə
            string passwordHash = _securityService.Hash(model.Password);

            var newUser = new UserServiceModel
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = passwordHash,
                Role = "Customer",  // həmişə Customer olacaq
                CreatedAt = DateTime.UtcNow
            };

            await _userService.SaveAsync(newUser);

            // Qeydiyyatdan sonra Login səhifəsinə yönləndir
            return RedirectToAction("Login", "Account");
        }
    }
}
