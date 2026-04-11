namespace Synkademy.DTOs;

public class AssignResearchAreasRequest
{
    public int SupervisorId { get; set; }
    public List<int> ResearchAreaIds { get; set; } = new();
}