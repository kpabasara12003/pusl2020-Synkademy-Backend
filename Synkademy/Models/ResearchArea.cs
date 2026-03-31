namespace Synkademy.Models
{
    public class ResearchArea
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<ProjectResearchArea> ProjectLinks { get; set; } = new List<ProjectResearchArea>();
        public ICollection<SupervisorResearchArea> SupervisorLinks { get; set; } = new List<SupervisorResearchArea>();
    }
}
