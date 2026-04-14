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
        public List<string> Tags { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public string? TechStack { get; set; }
        public string? Abstract { get; set; }
        public string? ProposalFilePath { get; set; }
    }

    public class SupervisorDropdownDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class AssignSupervisorRequest
    {
        public int ProjectId { get; set; }
        public int SupervisorId { get; set; }
    }

    public class BreakMatchRequest
    {
        public int ProjectId { get; set; }
    }
}