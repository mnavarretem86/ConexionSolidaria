using System.Collections.Generic;

namespace ConexionSolidaria.ViewModels
{
    public class DashboardVM
    {
        public List<DashboardEventoVM> Eventos { get; set; }
        public List<DashboardInscripcionVM> Inscripciones { get; set; }
    }
}
