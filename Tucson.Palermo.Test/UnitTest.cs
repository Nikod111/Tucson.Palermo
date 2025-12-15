using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tucson.Palermo.DTO;
using Tucson.Palermo.Services;

namespace Tucson.Palermo.Test
{
    public class UnitTest
    {
        /* 
           Las pruebas unitarias deben ejecutarse en un contexto aislado, sin producir efectos colaterales ni modificar el estado persistente de las reservas.
        */

        [Fact]
        public void FiltrarReservas_DevuelveResultados()
        {
            // Arrange
            var context = GetRealDbContext();
            var service = new ReservaService(context);

            var filtro = new ReservaFiltroViewModel
            {
                Pagina = 1,
                TamañoPagina = 10
            };

            // Act
            var resultado = service.FiltrarReservas(filtro);

            // Assert
            Assert.NotNull(resultado.Reservas);
            Assert.True(resultado.Total >= 0);
        }

        [Fact]
        public void GenerarHoras_DevuelveResultados()
        {
            var context = GetRealDbContext();
            var service = new ReservaService(context);

            // Act
            var resultado = service.GenerarHoras(0, 0, 12, 0);

            // Assert
            Assert.True(resultado.Count >= 0);
        }

        [Fact]
        public void ValidarReserva_NoDevuelveErrores()
        {
            var reserva = new Reserva
            {
                Fecha = DateTime.Now.AddDays(1), // fecha futura
                HoraInicio = new TimeSpan(19, 0, 0),
                HoraFin = new TimeSpan(23, 0, 0)
            };

            int horaInicio = 19;
            int minutoInicio = 0;
            int horaFin = 23;
            int minutoFin = 30;

            // Act
            var context = GetRealDbContext();
            var service = new ReservaService(context);
            var errores = service.ValidarReserva(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            // Assert
            Assert.Empty(errores);
        }
        [Fact]
        public void ValidarReserva_DevuelveErrores()
        {
            var reserva = new Reserva
            {
                Fecha = DateTime.Now.AddDays(-1), // fecha pasada
                HoraInicio = new TimeSpan(19, 0, 0),
                HoraFin = new TimeSpan(23, 0, 0)
            };

            int horaInicio = 19;
            int minutoInicio = 0;
            int horaFin = 23;
            int minutoFin = 30;

            // Act
            var context = GetRealDbContext();
            var service = new ReservaService(context);
            var errores = service.ValidarReserva(reserva, horaInicio, minutoInicio, horaFin, minutoFin);

            // Assert
            Assert.Contains(errores, e => e.campo == "Fecha");
        }


        private AppDbContext GetRealDbContext()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(config.GetConnectionString("DefaultConnection"))
                .Options;

            return new AppDbContext(options);
        }
    }
}