namespace Tucson.Palermo.DTO
{
    public class ReservaFiltroViewModel
    {
        public DateTime? Fecha { get; set; }
        public string? Tipo { get; set; }
        public string? Estado { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }

        public int Pagina { get; set; } = 1;
        public int TamañoPagina { get; set; } = 10;
    }

}
