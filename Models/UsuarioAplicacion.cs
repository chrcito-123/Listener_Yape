namespace Listener_Yape.Models
{
    public class UsuarioAplicacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int AplicacionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Usuario Usuario { get; set; }
        public Aplicacion Aplicacion { get; set; }
    }
}
