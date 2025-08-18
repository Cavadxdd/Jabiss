using Jabiss.Business.Services;
using Jabiss.Business.Services.Implementations;
using Jabiss.Business.Services.Interfaces;
using JabissStorage.DataAccess.Implementations;
using JabissStorage.DataAccess.Implementations.JabissStorage.DataAccess.Repositories;
using JabissStorage.Domain;
using JabissStorage.Domain.Entities;
using JabissStorage.Domain.Interfaces;
using Jabiss.Web.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Jabiss.Business.Services.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, SmtpEmailService>();

builder.Services.AddTransient<IUserVerificationRepository>(sp =>
    new MsSqlUserVerificationRepository(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddTransient<IUserVerificationService, UserVerificationService>();


builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
});

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme);

#region Identity Core for User 
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
})
.AddSignInManager<SignInManager<User>>()
.AddUserStore<UserStore>()
.AddDefaultTokenProviders();
#endregion

#region Hasher and Security Service
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
#endregion

#region Repositories and Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, MsSqlUserRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, MsSqlProductRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, MsSqlCategoryRepository>();
builder.Services.AddScoped<IProductImageRepository, MsSqlProductImageRepository>();
builder.Services.AddScoped<ICartRepository, MsSqlCartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICartItemRepository, MsSqlCartItemRepository>();
builder.Services.AddScoped<ICartService, CartService>();
#endregion

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
