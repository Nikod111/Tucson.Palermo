using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tucson.Palermo.DTO
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El email no es válido")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [NotMapped]
        public DateTime Fecha { get; set; }

        [Required]
        [NotMapped]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [NotMapped]
        public TimeSpan HoraFin { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaHoraInicio { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime FechaHoraFin { get; set; }

        [Required]
        [Range(1, 4, ErrorMessage = "La cantidad debe ser entre 1 y 4")]
        public int CantidadPersonas { get; set; }

        public string Estado { get; set; } = "Pendiente";

        [ValidateNever]
        public string Discriminator { get; set; }
    }
}