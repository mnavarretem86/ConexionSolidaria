using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConexionSolidaria.Models
{
    public class UsuarioViewModel
    {
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "La persona es obligatoria")]
        public int PersonaID { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = null!;

        [Required(ErrorMessage = "El rol es obligatorio")]
        public int RolID { get; set; }

        public int EstadoID { get; set; } = 1;

        [NotMapped]
        [ValidateNever]
        public string? NombreCompleto { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? Email { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? NombreRol { get; set; }
    }
}