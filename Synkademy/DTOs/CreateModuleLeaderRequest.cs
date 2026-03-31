namespace Synkademy.DTOs
{

    public class CreateStudentRequest
    {
        public int ModuleLeaderId { get; set; }
        public string StudentNumber { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateSupervisorRequest
    {
        public int ModuleLeaderId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CreateModuleLeaderRequest
    {
        public int ModuleLeaderId { get; set; } 
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
