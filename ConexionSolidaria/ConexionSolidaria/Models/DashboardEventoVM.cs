using System;

namespace ConexionSolidaria.ViewModels
{
    public class DashboardEventoVM
    {
        public int EventoID { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public string Lugar { get; set; }
        public int CupoMaximo { get; set; }
        public int CupoDisponible { get; set; }
        public string EstadoEvento { get; set; }
        public int TotalInscritos { get; set; }
    }
}