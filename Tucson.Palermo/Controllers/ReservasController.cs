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

        Validaciones(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

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

        Validaciones(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

        reserva.FechaHoraInicio = reserva.Fecha.Date + reserva.HoraInicio;
        reserva.FechaHoraFin = reserva.Fecha.Date + reserva.HoraFin;

        if (reserva.MesaPreferida.HasValue)
        {
            bool mesaOcupada = _context.ReservasVIP.Any(r =>
                r.MesaPreferida == reserva.MesaPreferida &&
                r.FechaHoraInicio < reserva.FechaHoraFin &&
                r.FechaHoraFin > reserva.FechaHoraInicio);

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

        Validaciones(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

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
        // Buscar la reserva
        var reserva = _reservaService.BuscarReserva(id);

        if (reserva == null)
            return NotFound();

        DateTime fechaReserva = reserva.FechaHoraInicio;

        if (fechaReserva < DateTime.Now)
        {
            TempData["Error"] = "No puede confirmarse una reserva con fecha pasada.";
            return View("Details");
        }

        if (reserva.Estado != "Pendiente" )
        {
            TempData["Error"] = "Solo puede confirmarse una reserva en estado pendiente.";
            return View("Details");
        }

        reserva.Estado = "Confirmada";
        _context.SaveChanges();

        TempData["Success"] = "La reserva fue confirmada exitosamente.";
        return View("Details");

    }

    [HttpPost]
    public IActionResult Cancelar(int id)
    {
        // Buscar la reserva
        var reserva = _reservaService.BuscarReserva(id);

        if (reserva == null)
            return NotFound();

        DateTime fechaReserva = reserva.FechaHoraInicio;

        if (reserva.Estado != "Pendiente" && reserva.Estado != "Confirmada")
        {
            TempData["Error"] = "Solo puede cancelarse una reserva en estado pendiente o confirmada.";
            return View("Details");
        }

        reserva.Estado = "Cancelada";
        _context.SaveChanges();

        TempData["Success"] = "La reserva fue cancelada exitosamente.";
        return View("Details");
    }

    [HttpPost]
    public IActionResult NoAsistio(int id)
    {
        // Buscar la reserva
        var reserva = _reservaService.BuscarReserva(id);

        if (reserva == null)
            return NotFound();

        if (reserva.Estado != "Confirmada")
        {
            TempData["Success"] = BadRequest(new { error = "Solo puede marcarse como 'No Asistió' a una reserva con estado confirmada" });
        }

        reserva.Estado = "No asistió";
        _context.SaveChanges();

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


    public IActionResult Confirmacion()
    {
        return View();
    }

}
