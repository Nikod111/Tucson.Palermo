using System.ComponentModel.DataAnnotations;

namespace Tucson.Palermo.DTO
{
    public class ReservaVIP : Reserva
    {
        [Required(ErrorMessage = "El código VIP es obligatorio")]
        [MinLength(6, ErrorMessage = "El código VIP debe tener al menos 6 caracteres")]
        public string CodigoVIP { get; set; }

        // Campo opcional
        [Range(1, 10, ErrorMessage = "La mesa debe ser entre 1 y 10")]
        public int MesaPreferida { get; set; }
    }
}
