using JabissStorage.DataAccess.Implementations;
using Microsoft.Extensions.Configuration;
using JabissStorage.Domain.Entities;
using JabissStorage.DataAccess.Implementations.JabissStorage.DataAccess.Repositories;
using Jabiss.Business.Services.Implementations;
using Jabiss.Business.Services.Models;

using Microsoft.Extensions.Configuration;
using JabissStorage.DataAccess.Implementations;
using Jabiss.Business.Services;
using Jabiss.Business.Services.Models;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

//// Repository ve Service'i oluştur
//var productRepo = new MsSqlProductRepository(config);
//var productService = new ProductService(productRepo);

//// ✔️ GetAll ürünleri getir
//var allProducts = await productService.GetAllAsync();

//Console.WriteLine("📦 Ürün Listesi:");
//foreach (var p in allProducts)
//{
//    Console.WriteLine($"Id: {p.Id}, Name: {p.Name}, Price: {p.Price}");
//}


#region Buisness.ProductServiceTest
#region SaveAsyncTest
//// ✔️ Yeni bir ürün ekle (Id = 0 olduğu için insert yapılır)
//var newProduct = new ProductServiceModel
//{
//    Id = 0,
//    Name = "Test",
//    Description = "Test",
//    Price = 99.99m,
//    Stock = 20,
//    CategoryId = 1,
//    ImageUrl = "test.jpg"
//};

//var isAdded = await productService.SaveAsync(newProduct);
//Console.WriteLine(isAdded ? "✅ Ürün eklendi." : "❌ Ekleme başarısız.");
#endregion
#region UpdateAsyncTest
//// ✔️ Güncelle (Id > 0 olduğu için update yapılır)
//var updatedProduct = new ProductServiceModel
//{
//    Id = 5, // Var olan bir ID gir
//    Name = "Gaming Chair",
//    Description = "Comforytable and fast construction",
//    Price = 99.99m,
//    Stock = 25,
//    CategoryId = 1,
//    ImageUrl = "https://example.com/images/gamingchair.jpg"
//};

//var isUpdated = await productService.SaveAsync(updatedProduct);
//Console.WriteLine(isUpdated ? "✅ Ürün güncellendi." : "❌ Güncelleme başarısız.");
#endregion
#region DeleteAsyncTest
//// ✔️ Silme
//var isDeleted = await productService.DeleteAsync(6); // Var olan bir ID gir
//Console.WriteLine(isDeleted ? "🗑️ Ürün silindi." : "❌ Silme başarısız.");
#endregion
#endregion


