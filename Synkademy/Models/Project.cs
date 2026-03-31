namespace Synkademy.Models
{
    public class Project
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Abstract { get; set; }
        public string? TechStack { get; set; }

        public int StudentId { get; set; }
        public int? SupervisorId { get; set; }

        public string? ProposalFilePath { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }

        // Navigation
        public Student Student { get; set; } = null!;
        public Employee? Supervisor { get; set; }

        public ICollection<ProjectResearchArea> ResearchAreas { get; set; } = new List<ProjectResearchArea>();
        public ICollection<ProjectTag> Tags { get; set; } = new List<ProjectTag>();
        public ICollection<ProjectInterest> Interests { get; set; } = new List<ProjectInterest>();
    }
}
