namespace Synkademy.DTOs
{
    public class CreateResearchAreaRequest
    {
        public string Name { get; set; } = string.Empty;
    }

    public class CreateProjectResearchAreaRequest
    {
        public int ProjectId { get; set; }
        public int ResearchAreaId { get; set; }
    }

    public class CreateProjectInterestRequest
    {
        public int ProjectId { get; set; }
        public int SupervisorId { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
