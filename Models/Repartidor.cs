namespace MASAMADREPROY.Models
{
    public class Repartidor
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public bool Disponible { get; set; } // Para saber si está disponible para un nuevo pedido
        public List<Pedido> PedidosAsignados { get; set; } // Pedidos asignados al repartidor
    }

}
