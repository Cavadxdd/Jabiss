using System.ComponentModel.DataAnnotations;

namespace Jabiss.Web.Models.Categories
{
    public class CreateCategoryViewModel
    {
        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
    }
}
