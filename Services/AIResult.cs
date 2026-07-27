namespace TechRent.Services
{
    public class AIResult
    {
        public string Respuesta { get; set; } = string.Empty;
        public string ModeloNombre { get; set; } = string.Empty;
        public long TiempoRespuestaMs { get; set; }
        public bool Exitoso { get; set; }
        public string? Error { get; set; }
    }
}
