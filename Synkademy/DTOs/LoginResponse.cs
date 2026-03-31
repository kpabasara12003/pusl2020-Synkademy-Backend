namespace Synkademy.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? StudentNumber { get; set; } // only for students
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
