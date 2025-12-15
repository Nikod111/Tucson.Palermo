using Microsoft.AspNetCore.Mvc.Rendering;
using Tucson.Palermo.DTO;

namespace Tucson.Palermo.Services
{
    public interface IReservaService
    {
        (List<Reserva> Reservas, int Total) FiltrarReservas(ReservaFiltroViewModel filtro);
        List<SelectListItem> GenerarHoras(int horaInicio, int minutoInicio, int horaFin, int minutoFin);
        public Reserva BuscarReserva(int id);
        (bool ok, string? error) ConfirmarReserva(int id);
        (bool ok, string? error) CancelarReserva(int id);
        (bool ok, string? error) MarcarNoAsistio(int id);
        List<(string campo, string mensaje)> ValidarReserva(Reserva reserva, int horaInicio, int minutoInicio, int horaFin, int minutoFin);    
    }

}
