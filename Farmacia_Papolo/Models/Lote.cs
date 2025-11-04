using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia_Paolo.Models
{
    public class Lote
    {
        [Key]
        public int LoteID { get; set; }

        [Required(ErrorMessage = "El número de lote es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Número de Lote")]
        public string NumeroLote { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser 0 o más")]
        [Display(Name = "Cantidad Actual")]
        public int CantidadActual { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Entrada")]
        public DateTime FechaEntrada { get; set; }

        // --- Relación con Producto ---
        [Display(Name = "Producto")]
        public int ProductoID { get; set; }

        // ⚠️ Aquí quita el [Required]
        [ForeignKey("ProductoID")]
        public virtual Producto? Producto { get; set; }

        // --- Relación con Movimientos ---
        // ⚠️ También hazla opcional (no [Required])
        public virtual ICollection<MovimientoInventario>? Movimientos { get; set; }

    }
}