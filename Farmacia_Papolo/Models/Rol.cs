using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Farmacia_Paolo.Models
{
    public class Rol
    {
        [Key]
        public int RolID { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(100)]
        [Display(Name = "Nombre del Rol")]
        public string NombreRol { get; set; }

        // --- Relación con Usuarios ---
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}