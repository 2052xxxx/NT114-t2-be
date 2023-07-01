namespace NT114_t2_be.Models
{
    public class Signup
    {
        public int Userid { get; set; }

        public string Username { get; set; } = null!;

        public string Realname { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
