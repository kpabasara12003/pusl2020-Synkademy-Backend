namespace Synkademy.DTOs
{
    public class CreateProjectRequest
    {
        public int StudentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Abstract { get; set; }
        public string? TechStack { get; set; }
        public string? ProposalFilePath { get; set; }
        // Use IDs for associated entities
        public List<int>? ResearchAreas { get; set; }
        public List<int>? Tags { get; set; }
    }

    public class UpdateProjectRequest
    {
        public int StudentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Abstract { get; set; }
        public string? TechStack { get; set; }
        public string? ProposalFilePath { get; set; }
        public List<int>? ResearchAreas { get; set; }
        public List<int>? Tags { get; set; }
    }

    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Abstract { get; set; }
        public string? TechStack { get; set; }
        public int StudentId { get; set; }
        public int? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public string? SupervisorEmail { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? ProposalFilePath { get; set; }
        public List<string> ResearchAreas { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
    }
}
