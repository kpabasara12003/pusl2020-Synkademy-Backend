namespace Synkademy.DTOs;

public class ProjectListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? TechStack { get; set; }
    public List<string> ResearchAreas { get; set; } = new();
}