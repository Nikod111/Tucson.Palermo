using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tucson.Palermo.DTO;

namespace Tucson.Palermo.Services
{
    public class ReservaService : IReservaService
    {
        private readonly AppDbContext _context;

        public ReservaService(AppDbContext context)
        {
            _context = context;
        }

        public (List<Reserva> Reservas, int Total) FiltrarReservas(ReservaFiltroViewModel filtro)
        {
            var query = _context.Reservas.AsQueryable();

            if (filtro.Fecha.HasValue)
                query = query.Where(r => r.FechaHoraInicio.Date == filtro.Fecha.Value.Date);

            if (!string.IsNullOrEmpty(filtro.Tipo))
                query = query.Where(r => r.Discriminator == filtro.Tipo);

            if (!string.IsNullOrEmpty(filtro.Estado))
                query = query.Where(r => r.Estado == filtro.Estado);

            if (!string.IsNullOrEmpty(filtro.Nombre))
                query = query.Where(r => r.Nombre.Contains(filtro.Nombre));

            if (!string.IsNullOrEmpty(filtro.Email))
                query = query.Where(r => r.Email.Contains(filtro.Email));

            int total = query.Count();

            var reservas = query
                .OrderBy(r => r.FechaHoraInicio)
                .Skip((filtro.Pagina - 1) * filtro.TamañoPagina)
                .Take(filtro.TamañoPagina)
                .ToList();

            return (reservas, total);
        }

        public List<SelectListItem> GenerarHoras(int horaInicio, int minutoInicio, int horaFin, int minutoFin)
        {
            var lista = new List<SelectListItem>();

            var inicio = new TimeSpan(horaInicio, minutoInicio, 0);
            var fin = new TimeSpan(horaFin, minutoFin, 0);

            // Caso normal: el fin es mayor o igual al inicio
            if (fin >= inicio)
            {
                for (var h = inicio; h <= fin; h = h.Add(TimeSpan.FromMinutes(30)))
                {
                    lista.Add(new SelectListItem
                    {
                        Value = h.ToString(@"hh\:mm"),
                        Text = h.ToString(@"hh\:mm")
                    });
                }
            }
            else
            {
                // Caso especial: cruza medianoche

                // 1) Desde inicio hasta 23:59
                for (var h = inicio; h < TimeSpan.FromDays(1); h = h.Add(TimeSpan.FromMinutes(30)))
                {
                    lista.Add(new SelectListItem
                    {
                        Value = h.ToString(@"hh\:mm"),
                        Text = h.ToString(@"hh\:mm")
                    });
                }

                // 2) Desde 00:00 hasta fin
                for (var h = TimeSpan.Zero; h <= fin; h = h.Add(TimeSpan.FromMinutes(30)))
                {
                    lista.Add(new SelectListItem
                    {
                        Value = h.ToString(@"hh\:mm"),
                        Text = h.ToString(@"hh\:mm")
                    });
                }
            }

            return lista;
        }

        public Reserva BuscarReserva(int id)
        {
            // Buscar la reserva
            var reserva = _context.Reservas
                .FirstOrDefault(r => r.Id == id);
            return reserva;

        }

        public (bool ok, string? error) ConfirmarReserva(int id)
        {
            var reserva = BuscarReserva(id);
            if (reserva == null)
                return (false, "Reserva no encontrada.");

            if (reserva.FechaHoraInicio < DateTime.Now)
                return (false, "No puede confirmarse una reserva con fecha pasada.");

            if (reserva.Estado != "Pendiente")
                return (false, "Solo puede confirmarse una reserva en estado pendiente.");

            reserva.Estado = "Confirmada";
            _context.SaveChanges();

            return (true, null);
        }

        public (bool ok, string? error) CancelarReserva(int id)
        {
            var reserva = BuscarReserva(id);
            if (reserva == null)
                return (false, "Reserva no encontrada.");

            if (reserva.Estado != "Pendiente" && reserva.Estado != "Confirmada")
                return (false, "Solo puede cancelarse una reserva en estado pendiente o confirmada.");

            reserva.Estado = "Cancelada";
            _context.SaveChanges();

            return (true, null);
        }

        public (bool ok, string? error) MarcarNoAsistio(int id)
        {
            var reserva = BuscarReserva(id);
            if (reserva == null)
                return (false, "Reserva no encontrada.");

            if (reserva.Estado != "Confirmada")
                return (false, "Solo puede marcarse como 'No Asistió' a una reserva con estado confirmada.");

            reserva.Estado = "No asistió";
            _context.SaveChanges();

            return (true, null);
        }

        public List<(string campo, string mensaje)> ValidarReserva(Reserva reserva, int horaInicio, int minutoInicio, int horaFin, int minutoFin)
        {
            var errores = new List<(string campo, string mensaje)>();

            if (reserva.Fecha.Date < DateTime.Now.Date)
            {
                errores.Add(("Fecha", "No se pueden crear reservas en fechas pasadas"));
            }

            var inicio = new TimeSpan(horaInicio, minutoInicio, 0);
            var fin = new TimeSpan(horaFin, minutoFin, 0);

            bool cruzaMedianoche = fin < inicio;

            TimeSpan hInicio = reserva.HoraInicio;
            TimeSpan hFin = reserva.HoraFin;

            if (cruzaMedianoche)
            {
                if (hFin < inicio)
                    hFin = hFin.Add(TimeSpan.FromHours(24));

                if (hInicio < inicio)
                    hInicio = hInicio.Add(TimeSpan.FromHours(24));

                fin = fin.Add(TimeSpan.FromHours(24));
            }

            if (hInicio < inicio || hInicio > fin)
            {
                errores.Add(("HoraInicio", $"El horario debe ser entre {inicio} y {fin}"));
            }

            if (hFin < inicio || hFin > fin)
            {
                errores.Add(("HoraFin", $"El horario debe ser entre {inicio} y {fin}"));
            }

            if (hFin <= hInicio)
            {
                errores.Add(("HoraFin", "La hora de fin debe ser mayor que la hora de inicio"));
            }

            return errores;
        }

    }

}
