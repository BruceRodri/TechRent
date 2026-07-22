namespace TechRent.Models
{
    public class DetalleOrdenAlquiler
    {
        public int Id { get; set; }
        public int OrdenAlquilerId { get; set; }
        public OrdenAlquiler OrdenAlquiler { get; set; } = null!;
        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; } = null!;
        public string NombreEquipo { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public int Dias { get; set; }
        public decimal PrecioPorDia { get; set; }
        public decimal Subtotal { get; set; }
    }
}
