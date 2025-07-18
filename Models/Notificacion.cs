namespace Listener_Yape.Models
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int AplicacionId { get; set; }
        public string MensajeCompleto { get; set; } = "";
        public string Persona { get; set; } = "";
        public decimal? Monto { get; set; }
        public DateTime FechaNotificacion { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Vista { get; set; } = false;
        public Usuario Usuario { get; set; }
        public Aplicacion Aplicacion { get; set; }
    }
}