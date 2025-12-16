using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConexionSolidaria.Models
{
    public class PersonaViewModel
    {
        public int PersonaID { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Segundo Nombre")]
        public string? SegundoNombre { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(50)]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = null!;

        [StringLength(50)]
        [Display(Name = "Segundo Apellido")]
        public string? SegundoApellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [StringLength(20)]
        public string DNI { get; set; } = null!;

        [Required(ErrorMessage = "El género es obligatorio")]
        [Display(Name = "Género")]
        public char Genero { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        public int Edad { get; set; }

        [EmailAddress(ErrorMessage = "Email no válido")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(200)]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [NotMapped]
        [ValidateNever]
        public string? NombreCompleto { get; set; }
    }
}
