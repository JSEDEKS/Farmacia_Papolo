using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmacia_Paolo.Models
{
    public class Producto
    {
        [Key]
        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El código (SKU) es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [StringLength(200)]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser 0 o más")]
        [Display(Name = "Stock Mínimo")]
        public int StockMinimo { get; set; }

        // --- Relación con Proveedor ---
        [Required(ErrorMessage = "Debe seleccionar un proveedor")]
        [Display(Name = "Proveedor")]
        public int ProveedorID { get; set; }

        [ForeignKey("ProveedorID")]
        public virtual Proveedor? Proveedor { get; set; }

        // --- Relación con Lotes ---
    
        public virtual ICollection<Lote>? Lotes { get; set; }
    }
}