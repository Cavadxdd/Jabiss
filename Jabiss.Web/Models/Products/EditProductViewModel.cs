using System.ComponentModel.DataAnnotations;

namespace Jabiss.Web.Models.Products
{
    public class EditProductViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be greater than zero.")]
        public int CategoryId { get; set; }

        public List<ProductImageViewModel> Images { get; set; } = new();

    }
    public class ProductImageViewModel
    {
        public int Id { get; set; } // DB id, 0 veya yoksa yeni resim
        public string ImageBase64 { get; set; } = string.Empty; // "data:image/jpeg;base64,...." veya sadece base64
    }
}
