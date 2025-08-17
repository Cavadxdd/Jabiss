using Microsoft.Extensions.Diagnostics.HealthChecks;

public class HomeListViewModel
{
        public required List<HomeProductModel> Products { get; set; } = new();
        public required List<HomeCategoryModel> Categories { get; set; } = new();

        public string? SortOption { get; set; } // 👈 Əlavə etdik
        public int? SelectedCategoryId { get; set; } // varsa, bu da
}

public class HomeProductModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public string? ImageBase64 { get; set; } // birinci şəkil üçün
}


public class HomeCategoryModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
