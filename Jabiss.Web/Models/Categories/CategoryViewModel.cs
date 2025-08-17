using JabissStorage.Domain.Entities;

namespace Jabiss.Web.Models.Categories
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            Categories = new List<CategoryModel>();
        }

        public List<CategoryModel> Categories { get; set; }
    }

    public class CategoryModel()
    {
        public  int Id { get; set; }
        public required string Name { get; set; }
    }
}
