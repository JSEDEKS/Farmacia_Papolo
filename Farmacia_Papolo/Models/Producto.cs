using System;
using System.ComponentModel.DataAnnotations;

namespace Farmacia_Paolo.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El fabricante es obligatorio")]
        [StringLength(100)]
        public string Fabricante { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        public int Cantidad { get; set; }

        [StringLength(200)]
        public string Categoria { get; set; }
    }
}
