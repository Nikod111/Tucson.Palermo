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
    }

}
