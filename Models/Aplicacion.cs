namespace Listener_Yape.Models
{
    public class Aplicacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string PackageName { get; set; }

        public List<Notificacion> Notificaciones { get; set; }
    }
}