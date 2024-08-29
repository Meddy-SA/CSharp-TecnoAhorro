namespace TecnoCredito.Models.Products;

public class Category
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public int? ParentCategoryId { get; set; }
  public virtual Category? ParentCategory { get; set; }
  public virtual ICollection<Category> SubCategories { get; set; } = [];
  public virtual ICollection<Product> Products { get; set; } = [];
}
