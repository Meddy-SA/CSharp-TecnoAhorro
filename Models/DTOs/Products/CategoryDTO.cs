namespace TecnoCredito.Models.DTOs.Products;

public record CategoryDTO
{
  public int Id { get; init; }
  public string Name { get; init; } = null!;
  public string Description { get; init; } = null!;
  public int? ParentCategoryId { get; init; }
  public ICollection<CategoryDTO> SubCategories { get; init; } = new List<CategoryDTO>();
}
