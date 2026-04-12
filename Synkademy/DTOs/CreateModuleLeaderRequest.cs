namespace Synkademy.DTOs
{

    public class CreateStudentRequest
    {
        public int ModuleLeaderId { get; set; }
        public required string StudentNumber { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class CreateSupervisorRequest
    {
        public int ModuleLeaderId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class CreateModuleLeaderRequest
    {
        public int ModuleLeaderId { get; set; } 
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
