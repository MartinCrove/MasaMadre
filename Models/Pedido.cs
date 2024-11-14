namespace MASAMADREPROY.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } // Relación con el cliente
        public DateTime FechaPedido { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public decimal CostoTotal { get; set; }
        public EstadoPedido Estado { get; set; }
        public List<DetallePedido> Detalles { get; set; } // Lista de productos en el pedido
    }

    public enum EstadoPedido
    {
        Preparando,
        EnCamino,
        Entregado,
        Cancelado
    }

}
