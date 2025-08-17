using System.ComponentModel.DataAnnotations;

namespace Jabiss.Web.Models.Categories
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = string.Empty;
    }
}
