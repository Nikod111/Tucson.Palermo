using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using Tucson.Palermo.DTO;
using Tucson.Palermo.Services;

public class ReservasController : Controller
{
    private readonly AppDbContext _context;
    private readonly IReservaService _reservaService;

    public ReservasController(AppDbContext context, IReservaService reservaService)
    {
        _context = context;
        _reservaService = reservaService;
    }

    [HttpGet]
    public IActionResult Crear()
    {
        ViewBag.Horas = _reservaService.GenerarHoras(19, 0, 23, 30);
        return View();
    }

    [HttpGet]
    public IActionResult CrearVIP()
    {
        ViewBag.Horas = _reservaService.GenerarHoras(12, 0, 01, 00);
        return View();
    }

    [HttpGet]
    public IActionResult CrearCumpleaños()
    {
        ViewBag.Horas = _reservaService.GenerarHoras(0, 0, 23, 00);
        return View();
    }

    public IActionResult Listado(ReservaFiltroViewModel filtro)
    {
        var resultado = _reservaService.FiltrarReservas(filtro);

        ViewBag.Total = resultado.Total;
        ViewBag.Filtro = filtro;

        return View(resultado.Reservas);
    }

    [HttpPost]
    public IActionResult Crear(Reserva reserva)
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
            ViewBag.Horas = _reservaService.GenerarHoras(horaInicio, minutoInicio, horaFin, minutoFin);
            return View(reserva);
        }

        reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
        reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;
        //reserva.Estado = "Pendiente";
        _context.Reservas.Add(reserva);
        _context.SaveChanges();

        return RedirectToAction("Confirmacion");
    }

    [HttpPost]
    public IActionResult CrearVIPDB(ReservaVIP reserva)
    {
        int horaInicio = 12;
        int minutoInicio = 0;
        int horaFin = 01;
        int minutoFin = 00;

        var errores = _reservaService.ValidarReserva(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

        foreach (var (campo, mensaje) in errores)
        {
            ModelState.AddModelError(campo, mensaje);
        }

        reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
        reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;

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
            ViewBag.Horas = _reservaService.GenerarHoras(horaInicio, minutoInicio, horaFin, minutoFin);
            return View("CrearVIP", reserva);
        }

        reserva.Estado = "Confirmada";
        _context.Reservas.Add(reserva);
        _context.SaveChanges();

        return RedirectToAction("Confirmacion");
    }

    [HttpPost]
    public IActionResult CrearCumpleañosDB(ReservaCumpleañero reserva)
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
            ViewBag.Horas = _reservaService.GenerarHoras(horaInicio, minutoInicio, horaFin, minutoFin);
            return View("CrearCumpleaños", reserva);
        }

        reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
        reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;
        reserva.Estado = "Pendiente";
        _context.Reservas.Add(reserva);
        _context.SaveChanges();

        return RedirectToAction("Confirmacion");
    }

    public IActionResult Detalle(int id)
    {
        var reserva = _context.Reservas.FirstOrDefault(r => r.Id == id);

        if (reserva == null)
            return NotFound();

        return View(reserva);
    }

    [HttpPost]
    public IActionResult Confirmar(int id)
    {
        var (ok, error) = _reservaService.ConfirmarReserva(id);

        if (!ok)
        {
            TempData["Error"] = error;
            return View("Details");
        }

        TempData["Success"] = "La reserva fue confirmada exitosamente.";
        return View("Details");
    }


    [HttpPost]
    public IActionResult Cancelar(int id)
    {
        var (ok, error) = _reservaService.CancelarReserva(id);

        if (!ok)
        {
            TempData["Error"] = error;
            return View("Details");
        }

        TempData["Success"] = "La reserva fue cancelada exitosamente.";
        return View("Details");
    }

    [HttpPost]
    public IActionResult NoAsistio(int id)
    {
        // Buscar la reserva
        var (ok, error) = _reservaService.MarcarNoAsistio(id);

        if (!ok)
        {
            TempData["Error"] = error;
            return View("Details");
        }

        TempData["Success"] = "La reserva se marcó como 'No asistió' exitosamente.";
        return View("Details");
    }

    public IActionResult Details(int id)
    {
        var reserva = _context.Reservas
            .FirstOrDefault(r => r.Id == id);

        if (reserva == null)
            return NotFound();

        return View(reserva);
    }

    public IActionResult Confirmacion()
    {
        return View();
    }

}
