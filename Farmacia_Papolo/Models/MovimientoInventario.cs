using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia_Paolo.Models
{
    public class MovimientoInventario
    {
        [Key]
        public int MovimientoID { get; set; }

        [Required]
        [Display(Name = "Tipo de Movimiento")]
        [StringLength(50)]
        public string TipoMovimiento { get; set; } // "Entrada", "Salida (Venta)", "Ajuste", "Merma"

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Cantidad { get; set; }

        [Required]
        [Display(Name = "Fecha del Movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [StringLength(500)]
        public string Motivo { get; set; } // "Venta #F-001", "Ajuste por vencimiento"

        // --- Relación con Lote ---
        [Required]
        [Display(Name = "Lote")]
        public int LoteID { get; set; }

        [ForeignKey("LoteID")]
        public virtual Lote Lote { get; set; }

        // --- Relación con Usuario ---
        [Required]
        [Display(Name = "Usuario")]
        public int UsuarioID { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}