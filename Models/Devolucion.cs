using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyect_Snake_West.Models
{
    [Table("t_devolucion")]
    public class Devolucion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Pedido { get; set; }

        [Required]
        public string Email { get; set; }

        public string Motivo { get; set; }

        public DateTime FechaSolicitud { get; set; }

        // Relación con Producto (si es necesario)
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
    }
}
