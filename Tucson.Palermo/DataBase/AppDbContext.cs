using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Tucson.Palermo.DTO;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<ReservaVIP> ReservasVIP { get; set; }
    public DbSet<ReservaCumpleañero> ReservasCumpleañero { get; set; }
}
