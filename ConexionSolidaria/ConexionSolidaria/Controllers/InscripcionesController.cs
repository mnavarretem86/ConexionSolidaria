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
    public class InscripcionesController : Controller
    {
        private readonly IConfiguration _config;

        public InscripcionesController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString => _config.GetConnectionString("DefaultConnection");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Inscribirse(int eventoId)
        {
            if (HttpContext.Session.GetString("Rol") != "Voluntario")
                return RedirectToAction("Index", "Eventos");

            int? voluntarioId = HttpContext.Session.GetInt32("VoluntarioID");
            if (voluntarioId == null) return RedirectToAction("Index", "Eventos");

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_INSCRIPCION", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 1);
                cmd.Parameters.AddWithValue("@EventoID", eventoId);
                cmd.Parameters.AddWithValue("@VoluntarioID", voluntarioId.Value);

                cn.Open();
                cmd.ExecuteNonQuery();

                TempData["Exito"] = "Te has inscrito correctamente al evento.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo completar la inscripción: " + ex.Message;
            }

            return RedirectToAction("Index", "Eventos");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Desinscribirse(int eventoId)
        {
            if (HttpContext.Session.GetString("Rol") != "Voluntario")
                return RedirectToAction("Index", "Eventos");

            int? voluntarioId = HttpContext.Session.GetInt32("VoluntarioID");
            if (voluntarioId == null) return RedirectToAction("Index", "Eventos");

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_INSCRIPCION", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 2);
                cmd.Parameters.AddWithValue("@EventoID", eventoId);
                cmd.Parameters.AddWithValue("@VoluntarioID", voluntarioId.Value);

                cn.Open();
                cmd.ExecuteNonQuery();

                TempData["Exito"] = "Te has desinscrito del evento correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la desinscripción: " + ex.Message;
            }

            return RedirectToAction("MisInscripciones");
        }

        public IActionResult MisInscripciones()
        {
            if (HttpContext.Session.GetString("Rol") != "Voluntario")
                return RedirectToAction("Index", "Eventos");

            int? voluntarioId = HttpContext.Session.GetInt32("VoluntarioID");
            if (voluntarioId == null) return RedirectToAction("Index", "Eventos");

            var lista = new List<MisInscripcionesVM>();

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_INSCRIPCION", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 3);
                cmd.Parameters.AddWithValue("@VoluntarioID", voluntarioId.Value);

                cn.Open();
                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new MisInscripcionesVM
                    {
                        EventoID = Convert.ToInt32(dr["EventoID"]),
                        NombreEvento = dr["Nombre"].ToString(),
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
                        Lugar = dr["Lugar"].ToString(),
                        FechaInscripcion = Convert.ToDateTime(dr["FechaInscripcion"]),
                        EstadoInscripcion = dr["Estado"].ToString(),
                        EstadoID = Convert.ToInt32(dr["EstadoID"])
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar tus inscripciones: " + ex.Message;
            }

            return View(lista);
        }
    }
}