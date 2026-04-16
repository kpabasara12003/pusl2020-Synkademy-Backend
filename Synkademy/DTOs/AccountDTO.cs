namespace Synkademy.DTOs
{
    public class AccountDTO
    {
        // Used to send the combined list to the frontend
        public class UserDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }

        // Used to catch the data from your Edit Modal
        public class UpdateUserRequest
        {
            public int Id { get; set; }
            public string Role { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string? NewPassword { get; set; } // Nullable, because they might leave it blank!
        }
    }
}
