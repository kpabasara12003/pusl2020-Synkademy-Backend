namespace Synkademy.Models
{
    public class SupervisorResearchArea
    {
        public int SupervisorId { get; set; }
        public int ResearchAreaId { get; set; }

        public Employee Supervisor { get; set; } = null!;
        public ResearchArea ResearchArea { get; set; } = null!;
    }
}
