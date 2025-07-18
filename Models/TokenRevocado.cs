public class TokenRevocado
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaRevocado { get; set; } = DateTime.UtcNow;
}
