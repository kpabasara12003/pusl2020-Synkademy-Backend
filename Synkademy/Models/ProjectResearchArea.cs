namespace Synkademy.Models
{
    public class ProjectResearchArea
    {
        public int ProjectId { get; set; }
        public int ResearchAreaId { get; set; }

        public Project Project { get; set; } = null!;
        public ResearchArea ResearchArea { get; set; } = null!;
    }
}
