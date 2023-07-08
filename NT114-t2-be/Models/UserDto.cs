using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT114_t2_be.Models
{
    public class UserDto
    {
       public int Userid { get; set; }
        public string? Username { get; set; } = string.Empty;
        //[DefaultValue("")]
        public string? Realname { get; set; } = string.Empty;
        [NotMapped]
        public IFormFile? avatar { get; set; }
        public string? Bio { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }

    }
}
