namespace Listener_Yape.Models.Requests
{
    public class NotificacionRequest
    {
        public string PackageName { get; set; }
        public string MensajeCompleto { get; set; }
        public DateTime FechaNotificacion { get; set; }
    }
}