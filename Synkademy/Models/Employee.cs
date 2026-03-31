using System.ComponentModel.DataAnnotations;

namespace Synkademy.Models
{
    public class Employee
    {

        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty; // Supervisor / ModuleLeader

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Project> SupervisedProjects { get; set; } = new List<Project>();
        public ICollection<ProjectInterest> Interests { get; set; } = new List<ProjectInterest>();
        public ICollection<SupervisorResearchArea> ResearchAreas { get; set; } = new List<SupervisorResearchArea>();
    }
}
