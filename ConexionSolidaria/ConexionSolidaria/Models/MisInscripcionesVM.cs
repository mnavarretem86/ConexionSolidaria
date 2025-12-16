using System;

namespace ConexionSolidaria.ViewModels
{
    public class MisInscripcionesVM
    {
        public int EventoID { get; set; }
        public string NombreEvento { get; set; }
        public DateTime Fecha { get; set; }
        public string Lugar { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string EstadoInscripcion { get; set; }
        public int EstadoID { get; set; }
    }
}
