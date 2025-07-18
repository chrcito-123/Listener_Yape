using System.Text.RegularExpressions;

namespace Listener_Yape.Services
{
    public static class NotificationParser
    {
        public static (string Persona, decimal? Monto) Parse(string packageName, string mensajeCompleto)
        {
            if (packageName == "com.bcp.innovacxion.yapeapp" && mensajeCompleto.Contains("Yape"))
            {
                try
                {
                    // Nuevo formato: "Jorge F. Flores M. te envió un pago por S/ 0.1. El cód. de seguridad es: 806"
                    var match = Regex.Match(mensajeCompleto, @"^(.*?)\s+te envió un pago por S/\s*([\d.]+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        var nombre = match.Groups[1].Value.Trim();
                        var montoStr = match.Groups[2].Value;
                        // Eliminar "Yape!" al inicio si está presente
                        if (nombre.StartsWith("Yape!", StringComparison.OrdinalIgnoreCase))
                            nombre = nombre.Substring(5).TrimStart();

                        if (decimal.TryParse(montoStr, out decimal monto))
                            return (nombre, monto);
                    }
                }
                catch
                {
                    return ("Desconocido", null);
                }
            }
            else if (packageName == "com.plin.plinapp" && mensajeCompleto.Contains("Plin"))
            {
                return ("PlinUsuario", null);
            }

            return ("Desconocido", null);
        }
    }

}
