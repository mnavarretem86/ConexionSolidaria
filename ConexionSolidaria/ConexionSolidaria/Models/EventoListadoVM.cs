using System;

namespace ConexionSolidaria.ViewModels
{
    public class EventoListadoVM
    {
        public int EventoID { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }
        public string Lugar { get; set; }
        public int CupoDisponible { get; set; }
        public int CupoMaximo { get; set; }
        public int EstadoID { get; set; }
        public bool EstaInscrito { get; set; }
    }
}
