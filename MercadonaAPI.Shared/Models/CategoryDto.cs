namespace MercadonaAPI.Shared.Models;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ParentId { get; set; } = 0;

    public CategoryDto()
    {
    }
    public CategoryDto(int id, string name, int parentId)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
    }
}
