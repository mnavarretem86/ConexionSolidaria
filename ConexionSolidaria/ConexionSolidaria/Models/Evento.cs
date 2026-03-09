using System;

namespace ConexionSolidaria.Models
{
    public class Evento
    {
        public int EventoID { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public string Lugar { get; set; }
        public int CupoMaximo { get; set; }
        public int CupoDisponible { get; set; }
        public int UsuarioCreadorID { get; set; }
        public int EstadoID { get; set; }
    }
}