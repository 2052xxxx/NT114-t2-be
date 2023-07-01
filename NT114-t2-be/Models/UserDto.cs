using System.ComponentModel;

namespace NT114_t2_be.Models
{
    public class UserDto
    {
        public int Userid { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? Realname { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = null!;
        public IFormFile? formFile { get; set; }
        public string? Bio { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }

    }
}
