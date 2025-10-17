namespace BuildM.Models
{
    public class Detalle
    {
        public int IdDetalle { get; set; }
        public int IdSolicitud { get; set; }
        public int IdMaterial { get; set; }
        public string NombreProyecto { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string NombreMaterial { get; set; } = string.Empty;
        public string DescripcionMaterial { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
