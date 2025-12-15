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

            var errores = _reservaService.ValidarReserva(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            foreach (var (campo, mensaje) in errores)
            {
                ModelState.AddModelError(campo, mensaje);
            }

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

            var errores = _reservaService.ValidarReserva(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            foreach (var (campo, mensaje) in errores)
            {
                ModelState.AddModelError(campo, mensaje);
            }

            if (reserva.MesaPreferida.HasValue)
            {
                bool mesaOcupada = _context.ReservasVIP.Any(r =>
                    r.MesaPreferida == reserva.MesaPreferida &&
                    r.FechaHoraInicio < reserva.FechaHoraFin &&
                    r.FechaHoraFin > reserva.FechaHoraInicio &&
                    (r.Estado == "Confirmada" || r.Estado == "Pendiente"));

                if (mesaOcupada)
                {
                    ModelState.AddModelError("MesaPreferida", $"La mesa {reserva.MesaPreferida} no está disponible en ese horario.");
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

            var errores = _reservaService.ValidarReserva(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            foreach (var (campo, mensaje) in errores)
            {
                ModelState.AddModelError(campo, mensaje);
            }

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
            var (ok, error) = _reservaService.ConfirmarReserva(id);

            if (!ok)
                return BadRequest(new { error });

            return Ok(new { id, estado = "Confirmada" });

        }

        [HttpPost("cancelar-reserva")]
        public IActionResult Cancelar(int id)
        {
            var (ok, error) = _reservaService.CancelarReserva(id);

            if (!ok)
                return BadRequest(new { error });

            return Ok(new { id, estado = "Cancelada" });
        }

        [HttpPost("no-asistio-reserva")]
        public IActionResult NoAsistio(int id)
        {
            var (ok, error) = _reservaService.MarcarNoAsistio(id);

            if (!ok)
                return BadRequest(new { error });

            return Ok(new { id, estado = "No asistió" });
        }
    }

}
