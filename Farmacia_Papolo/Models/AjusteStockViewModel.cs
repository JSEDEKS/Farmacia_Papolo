using System.ComponentModel.DataAnnotations;

namespace Farmacia_Paolo.Models
{
    public class AjusteStockViewModel
    {
        [Required]
        public int LoteID { get; set; }

        [Display(Name = "Producto")]
        public string? ProductoNombre { get; set; }

        [Display(Name = "Número de Lote")]
        public string? LoteNumero { get; set; }

        [Display(Name = "Cantidad Actual")]
        public int CantidadActual { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo de movimiento")]
        [Display(Name = "Tipo de Ajuste")]
        public string? TipoMovimiento { get; set; } // "Merma", "Ajuste Negativo", "Ajuste Positivo"

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad a Ajustar")]
        public int CantidadAjuste { get; set; }

        [Required(ErrorMessage = "Debe especificar un motivo")]
        [StringLength(500)]
        public string? Motivo { get; set; }
    }
}