using System.ComponentModel.DataAnnotations;

namespace Synkademy.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        public string StudentNumber { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
