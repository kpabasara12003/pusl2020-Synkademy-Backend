namespace Synkademy.DTOs
{
    public class SupervisorResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<string> ResearchAreas { get; set; } = new List<string>();
        public int SupervisedProjectsCount { get; set; }
    }
}
