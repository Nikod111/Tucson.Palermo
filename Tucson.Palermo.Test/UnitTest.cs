using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tucson.Palermo.DTO;
using Tucson.Palermo.Services;

namespace Tucson.Palermo.Test
{
    public class UnitTest
    {
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