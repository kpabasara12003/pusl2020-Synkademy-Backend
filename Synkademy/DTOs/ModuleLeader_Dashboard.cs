using System.Collections.Generic;

namespace Synkademy.DTOs
{
    public class DashboardResponseDto
    {
        public int TotalProposals { get; set; }
        public int MatchedProjects { get; set; }
        public int PendingReview { get; set; }
        public List<ProjectDirectoryDto> Projects { get; set; } = new();

        public List<string> AvailableResearchAreas { get; set; } = new();
        public List<SupervisorDropdownDto> AvailableSupervisors { get; set; } = new();
    }

    public class ProjectDirectoryDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> ResearchAreas { get; set; } = new();
        public string StudentName { get; set; } = string.Empty;
        public string SupervisorName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // A small DTO just for the supervisor dropdown
    public class SupervisorDropdownDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}