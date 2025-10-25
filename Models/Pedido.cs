using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyect_Snake_West.Models
{
    [Table("t_order", Schema = "public")]
    public class Pedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int ID { get; set; }

        [Column("userid")]
        public string? UserID { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        // Relación 1..* con DetallePedido
        public List<DetallePedido> Detalles { get; set; } = new();
    }
}
