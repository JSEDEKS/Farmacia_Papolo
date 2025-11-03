using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Farmacia_Paolo.Models
{
    public class Proveedor
    {
        [Key]
        public int ProveedorID { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio")]
        [StringLength(255)]
        [Display(Name = "Nombre del Proveedor")]
        public string NombreProveedor { get; set; }

        [StringLength(50)]
        [Display(Name = "RNC")]
        public string RNC { get; set; }

        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        // --- Relación con Productos ---
        // Un proveedor puede suplir muchos productos
        public virtual ICollection<Producto> Productos { get; set; }
    }
}