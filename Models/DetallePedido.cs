using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Proyect_Snake_West.Models
{
    [Table("t_order_detail", Schema = "public")]
    public class DetallePedido
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int ID { get; set; }

        // FK a Producto (si tu tabla t_producto tiene PK "id")
        [Column("productoid")]
        public int ProductoId { get; set; }

        // FK a Pedido
        [Column("pedidoid")]
        public int PedidoID { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        // Navegación (opcional, para Include)
        public Producto? Producto { get; set; }
        public Pedido? Pedido { get; set; }
    }
}
