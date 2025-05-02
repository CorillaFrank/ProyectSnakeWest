using System.ComponentModel.DataAnnotations.Schema;

namespace Proyect_Snake_West.Models
{
    [Table("t_order")]
    public class Pedido
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int ID { get; set; }

        public string? UserID { get; set; }

        public decimal Total { get; set; }

        public Pago? pago { get; set; }

        public string? Status { get; set; }

        // Relación uno a muchos: un pedido tiene muchos detalles
        public List<DetallePedido> Detalles { get; set; } = new();
    }
}
