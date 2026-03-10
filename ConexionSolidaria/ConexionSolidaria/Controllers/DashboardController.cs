using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ConexionSolidaria.ViewModels;

namespace ConexionSolidaria.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _config;

        public DashboardController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString =>
            _config.GetConnectionString("DefaultConnection");

        public IActionResult Index(string search, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            if (HttpContext.Session.GetString("Rol") != "Coordinador")
                return RedirectToAction("Index", "Home");

            var todosLosEventos = new List<DashboardEventoVM>();
            var todasLasInscripciones = new List<DashboardInscripcionVM>();

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_DASHBOARD", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Opcion", 1);
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    todosLosEventos.Add(new DashboardEventoVM
                    {
                        EventoID = Convert.ToInt32(dr["EventoID"]),
                        Nombre = dr["Nombre"].ToString(),
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
                        HoraInicio = (TimeSpan)dr["HoraInicio"],
                        HoraFin = (TimeSpan)dr["HoraFin"],
                        Lugar = dr["Lugar"].ToString(),
                        CupoMaximo = Convert.ToInt32(dr["CupoMaximo"]),
                        CupoDisponible = Convert.ToInt32(dr["CupoDisponible"]),
                        EstadoEvento = dr["EstadoEvento"].ToString(),
                        TotalInscritos = Convert.ToInt32(dr["TotalInscritos"])
                    });
                }
            }

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Opcion", 2);
            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    todasLasInscripciones.Add(new DashboardInscripcionVM
                    {
                        NombreVoluntario = dr["NombreVoluntario"].ToString(),
                        Evento = dr["Evento"].ToString(),
                        FechaEvento = Convert.ToDateTime(dr["FechaEvento"]),
                        HoraInicio = (TimeSpan)dr["HoraInicio"],
                        HoraFin = (TimeSpan)dr["HoraFin"],
                        FechaInscripcion = Convert.ToDateTime(dr["FechaInscripcion"]),
                        EstadoInscripcion = dr["EstadoInscripcion"].ToString()
                    });
                }
            }

            var eventosFiltrados = todosLosEventos.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                eventosFiltrados = eventosFiltrados.Where(e =>
                    e.Nombre.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Lugar.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                eventosFiltrados = eventosFiltrados.Where(e => e.EstadoEvento == estado);
            }

            if (fechaInicio.HasValue)
            {
                eventosFiltrados = eventosFiltrados.Where(e => e.Fecha.Date >= fechaInicio.Value.Date);
            }
            if (fechaFin.HasValue)
            {
                eventosFiltrados = eventosFiltrados.Where(e => e.Fecha.Date <= fechaFin.Value.Date);
            }

            var listaEventosFinal = eventosFiltrados.ToList();

            var nombresEventosFiltrados = listaEventosFinal.Select(e => e.Nombre).ToList();

            var inscripcionesFiltradas = todasLasInscripciones
                .Where(i => nombresEventosFiltrados.Contains(i.Evento))
                .ToList();

            var model = new DashboardVM
            {
                Eventos = listaEventosFinal,
                Inscripciones = inscripcionesFiltradas
            };

            return View(model);
        }
    }
}