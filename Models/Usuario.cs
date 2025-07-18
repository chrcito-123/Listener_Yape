namespace Listener_Yape.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Nombre { get; set; }
    }
}
