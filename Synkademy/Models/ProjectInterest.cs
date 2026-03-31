namespace Synkademy.Models
{
    public class ProjectInterest
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int SupervisorId { get; set; }

        public string Status { get; set; } = "Interested";

        public DateTime CreatedAt { get; set; }

        public Project Project { get; set; } = null!;
        public Employee Supervisor { get; set; } = null!;
    }
}
