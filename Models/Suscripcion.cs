namespace Listener_Yape.Models
{
    public class Suscripcion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Tipo { get; set; } = "";
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "activa";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
