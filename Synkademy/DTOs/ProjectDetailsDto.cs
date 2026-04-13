namespace Synkademy.DTOs
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Abstract { get; set; }
        public string? TechStack { get; set; }

        public List<string> ResearchAreas { get; set; } = new();
        public List<string> Tags { get; set; } = new();

        public DateTime CreatedAt { get; set; }
    }
}