using JabissStorage.Domain.Entities;

namespace Jabiss.Web.Models.Products
{
    public class ProductViewModel
    {
        public List<ProductModel> Products { get; set; } = new List<ProductModel>();
    }

    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public List<ProductImageModel> Images { get; set; } = new List<ProductImageModel>();
    }

    public class ProductImageModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public byte[] ImageData { get; set; }
    }
}
