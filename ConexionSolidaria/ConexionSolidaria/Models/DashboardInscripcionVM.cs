using System;

namespace ConexionSolidaria.ViewModels
{
    public class DashboardInscripcionVM
    {
        public string NombreVoluntario { get; set; }
        public string Evento { get; set; }
        public DateTime FechaEvento { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string EstadoInscripcion { get; set; }
    }
}
