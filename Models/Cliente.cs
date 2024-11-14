namespace MASAMADREPROY.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public List<Pedido> HistorialPedidos { get; set; } // Lista de pedidos realizados por el cliente
    }

}
