using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
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

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Rol") != "Coordinador")
                return RedirectToAction("Index", "Home");

            var model = new DashboardVM
            {
                Eventos = new List<DashboardEventoVM>(),
                Inscripciones = new List<DashboardInscripcionVM>()
            };

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
                    model.Eventos.Add(new DashboardEventoVM
                    {
                        EventoID = Convert.ToInt32(dr["EventoID"]),
                        Nombre = dr["Nombre"].ToString(),
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
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
                    model.Inscripciones.Add(new DashboardInscripcionVM
                    {
                        NombreVoluntario = dr["NombreVoluntario"].ToString(),
                        Evento = dr["Evento"].ToString(),
                        FechaEvento = Convert.ToDateTime(dr["FechaEvento"]),
                        FechaInscripcion = Convert.ToDateTime(dr["FechaInscripcion"]),
                        EstadoInscripcion = dr["EstadoInscripcion"].ToString()
                    });
                }
            }

            return View(model);
        }
    }
}
