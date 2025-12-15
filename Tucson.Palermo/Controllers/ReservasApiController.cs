using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tucson.Palermo.DTO;
using Tucson.Palermo.Services;

namespace Tucson.Palermo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasApiController : ControllerBase
    {
        /* 
           Se decidió implementar un controlador separado llamado ReservasAPI para mantener intacto el controlador MVC y evitar mezclarlo con validaciones personalizadas.
        */

        private readonly AppDbContext _context;
        private readonly IReservaService _reservaService;

        public ReservasApiController(AppDbContext context, IReservaService reservaService)
        {
            _context = context;
            _reservaService = reservaService;
        }

        [HttpGet("listado")]
        public IActionResult Listado([FromQuery] ReservaFiltroViewModel filtro)
        {
            var resultado = _reservaService.FiltrarReservas(filtro);

            return Ok(new
            {
                total = resultado.Total,
                filtro = filtro,
                reservas = resultado.Reservas
            });
        }


        [HttpPost("crear")]
        public IActionResult Crear([FromBody] Reserva reserva)
        {
            int horaInicio = 19;
            int minutoInicio = 0;
            int horaFin = 23;
            int minutoFin = 30;

            Validaciones(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            if (!ModelState.IsValid)
            {
                // En API devolvés errores de validación
                return BadRequest(ModelState);
            }

            reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
            reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;
            reserva.Estado = "Pendiente";

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            return Ok(new { id = reserva.Id });
        }

        [HttpPost("crear-vip")]
        public IActionResult CrearVIP([FromBody] ReservaVIP reserva)
        {
            int horaInicio = 12;
            int minutoInicio = 0;
            int horaFin = 1;
            int minutoFin = 0;

            Validaciones(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            if (reserva.MesaPreferida.HasValue)
            {
                bool mesaOcupada = _context.ReservasVIP.Any(r =>
                    r.MesaPreferida == reserva.MesaPreferida &&
                    r.FechaHoraInicio < reserva.FechaHoraFin &&
                    r.FechaHoraFin > reserva.FechaHoraInicio &&
                    (r.Estado == "Confirmada" || r.Estado == "Pendiente"));

                if (mesaOcupada)
                {
                    return BadRequest(new { error = $"La mesa {reserva.MesaPreferida} no está disponible en ese horario." });
                }
            }

            if (!ModelState.IsValid)
            {
                // En API devolvés errores de validación
                return BadRequest(ModelState);
            }

            reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
            reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;
            reserva.Estado = "Confirmada";

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            return Ok(new { id = reserva.Id });
        }

        [HttpPost("crear-cumpleaños")]
        public IActionResult CrearCumpleaños(ReservaCumpleañero reserva)
        {
            int horaInicio = 00;
            int minutoInicio = 0;
            int horaFin = 23;
            int minutoFin = 00;

            // Si requiere torta, la reserva debe realizarse mínimo 48 horas antes

            if (reserva.RequiereTorta)
            {
                if (reserva.Fecha.Date + reserva.HoraInicio < DateTime.Now.AddHours(48))
                {
                    ModelState.AddModelError("RequiereTorta", "Si requiere torta, la reserva debe realizarse con al menos 48 horas de anticipación");
                }
            }

            Validaciones(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
            reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;
            reserva.Estado = "Pendiente";
            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            return Ok(new { id = reserva.Id });
        }

        [HttpGet("detalle")]
        public IActionResult Detalle(int id)
        {
            var reserva = _context.Reservas.FirstOrDefault(r => r.Id == id);

            if (reserva == null)
                return NotFound();

            return Ok(reserva);
        }

        [HttpPost("confirmar-reserva")]
        public IActionResult Confirmar(int id)
        {
            // Buscar la reserva
            var reserva = _reservaService.BuscarReserva(id);

            if (reserva == null)
                return NotFound();

            DateTime fechaReserva = reserva.FechaHoraInicio;

            if (fechaReserva < DateTime.Now)
            {
                return BadRequest(new { error = "No puede confirmarse una reserva con fecha pasada." });
            }

            if (reserva.Estado != "Pendiente" )
            {
                return BadRequest(new { error = "Solo puede confirmarse una reserva en estado pendiente." });
            }

            reserva.Estado = "Confirmada";
            _context.SaveChanges();

            return Ok("La reserva fue confirmada exitosamente.");

        }

        [HttpPost("cancelar-reserva")]
        public IActionResult Cancelar(int id)
        {
            // Buscar la reserva
            var reserva = _reservaService.BuscarReserva(id);

            if (reserva == null)
                return NotFound();

            DateTime fechaReserva = reserva.FechaHoraInicio;
            if (fechaReserva < DateTime.Now)
            {
                return BadRequest(new { error = "No puede confirmarse una reserva con fecha pasada." });
            }

            if (reserva.Estado != "Pendiente" && reserva.Estado != "Confirmada" )
            {
                return BadRequest(new { error = "Solo puede cancelarse una reserva en estado pendiente o confirmada." });
            }   

            reserva.Estado = "Cancelada";
            _context.SaveChanges();

            return Ok("La reserva fue cancelada exitosamente.");
        }

        [HttpPost("no-asistio-reserva")]
        public IActionResult NoAsistio(int id)
        {
            // Buscar la reserva
            var reserva = _reservaService.BuscarReserva(id);

            if (reserva == null)
                return NotFound();

            if (reserva.Estado != "Confirmada")
            {
                return BadRequest(new { error = "Solo puede marcarse como 'No Asistió' a una reserva con estado confirmada" });
            }

            reserva.Estado = "No asistió";
            _context.SaveChanges();

            return Ok("La reserva se marcó como 'No asistió' exitosamente");
        }

        private void Validaciones(Reserva reserva, int horaInicio, int minutoInicio, int horaFin, int minutoFin)
        {
            if (reserva.Fecha.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Fecha", "No se pueden crear reservas en fechas pasadas");
            }

            var inicio = new TimeSpan(horaInicio, minutoInicio, 0);
            var fin = new TimeSpan(horaFin, minutoFin, 0);

            bool cruzaMedianoche = fin < inicio;

            // Normalizar horas del usuario
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

            // Validación de rango permitido
            if (hInicio < inicio || hInicio > fin)
            {
                ModelState.AddModelError("HoraInicio", $"El horario debe ser entre {inicio} y {fin}");
            }

            if (hFin < inicio || hFin > fin)
            {
                ModelState.AddModelError("HoraFin", $"El horario debe ser entre {inicio} y {fin}");
            }

            // Validación inicio < fin
            if (hFin <= hInicio)
            {
                ModelState.AddModelError("HoraFin", "La hora de fin debe ser mayor que la hora de inicio");
            }
        }
    }

}
