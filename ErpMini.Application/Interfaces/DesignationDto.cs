// ErpMini.Application/DTOs/DesignationDto.cs
namespace ErpMini.Application.DTOs;

public class DesignationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}