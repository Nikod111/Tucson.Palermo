using System.ComponentModel.DataAnnotations;

namespace Tucson.Palermo.DTO
{
    public class ReservaCumpleañero : Reserva
    {
        [Required]
        public int EdadCumpleañero { get; set; }

        public bool RequiereTorta { get; set; }

        [Required]
        [Range(5, 12, ErrorMessage = "La cantidad debe ser entre 5 y 12")]
        public int CantidadPersonas { get; set; }
    }
}
