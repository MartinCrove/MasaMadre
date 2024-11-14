namespace MASAMADREPROY.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Imagen { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CantidadEnStock { get; set; }
        public bool Disponible { get; set; } // Para indicar si está disponible para venta
    }

}
