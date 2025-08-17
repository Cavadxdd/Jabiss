using System.ComponentModel.DataAnnotations;

namespace Jabiss.Web.Models.Products
{
    public class CreateProductViewModel
    {

        public CreateProductViewModel()
        {
            Images = new List<ProductImageModel>();
        }

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

        public  List<ProductImageModel> Images { get; set; }
    }
}


