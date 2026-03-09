using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using ConexionSolidaria.Models;
using ConexionSolidaria.ViewModels;

namespace ConexionSolidaria.Controllers
{
    public class EventosController : Controller
    {
        private readonly IConfiguration _config;

        public EventosController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString =>
            _config.GetConnectionString("DefaultConnection");

        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UsuarioID") == null)
                return RedirectToAction("Index", "Home");

            var lista = new List<EventoListadoVM>();
            var rol = HttpContext.Session.GetString("Rol");
            var voluntarioIdSession = HttpContext.Session.GetInt32("VoluntarioID");

            object voluntarioParam = (rol == "Coordinador")
                ? DBNull.Value
                : (object)voluntarioIdSession ?? DBNull.Value;

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_EVENTO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 3);
                cmd.Parameters.AddWithValue("@VoluntarioID", voluntarioParam);

                cn.Open();
                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new EventoListadoVM
                    {
                        EventoID = Convert.ToInt32(dr["EventoID"]),
                        Nombre = dr["Nombre"].ToString(),
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
                        HoraInicio = (TimeSpan)dr["HoraInicio"],
                        HoraFin = (TimeSpan)dr["HoraFin"],
                        Lugar = dr["Lugar"].ToString(),
                        CupoMaximo = Convert.ToInt32(dr["CupoMaximo"]),
                        CupoDisponible = Convert.ToInt32(dr["CupoDisponible"]),
                        EstadoID = Convert.ToInt32(dr["EstadoID"]),
                        EstaInscrito = Convert.ToInt32(dr["EstaInscrito"]) == 1
                    });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar eventos: " + ex.Message;
            }

            return View(lista);
        }

        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("Rol") != "Coordinador")
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Evento model)
        {
            if (HttpContext.Session.GetString("Rol") != "Coordinador")
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            var usuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (usuarioId == null)
                return RedirectToAction("Index", "Home");

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_EVENTO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 1);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Fecha", model.Fecha);
                cmd.Parameters.AddWithValue("@HoraInicio", model.HoraInicio);
                cmd.Parameters.AddWithValue("@HoraFin", model.HoraFin);
                cmd.Parameters.AddWithValue("@Lugar", model.Lugar);
                cmd.Parameters.AddWithValue("@CupoMaximo", model.CupoMaximo);
                cmd.Parameters.AddWithValue("@UsuarioCreadorID", usuarioId.Value);

                cn.Open();

                using var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (dr["Error"] != DBNull.Value)
                    {
                        TempData["Error"] = dr["Error"].ToString();
                        return View(model);
                    }

                    if (dr["Mensaje"] != DBNull.Value)
                    {
                        TempData["Exito"] = dr["Mensaje"].ToString();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo crear el evento: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult Editar(int id)
        {
            if (HttpContext.Session.GetString("Rol") != "Coordinador")
                return RedirectToAction("Index");

            Evento evento = null;

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_EVENTO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 3);
                cmd.Parameters.AddWithValue("@VoluntarioID", DBNull.Value);

                cn.Open();
                using var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (Convert.ToInt32(dr["EventoID"]) == id)
                    {
                        evento = new Evento
                        {
                            EventoID = id,
                            Nombre = dr["Nombre"].ToString(),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            HoraInicio = (TimeSpan)dr["HoraInicio"],
                            HoraFin = (TimeSpan)dr["HoraFin"],
                            Lugar = dr["Lugar"].ToString(),
                            CupoMaximo = Convert.ToInt32(dr["CupoMaximo"]),
                            EstadoID = Convert.ToInt32(dr["EstadoID"])
                        };
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al buscar el evento: " + ex.Message;
            }

            if (evento == null)
                return NotFound();

            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Evento model)
        {
            if (HttpContext.Session.GetString("Rol") != "Coordinador")
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_EVENTO", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Opcion", 2);
                cmd.Parameters.AddWithValue("@EventoID", model.EventoID);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre);
                cmd.Parameters.AddWithValue("@Fecha", model.Fecha);
                cmd.Parameters.AddWithValue("@HoraInicio", model.HoraInicio);
                cmd.Parameters.AddWithValue("@HoraFin", model.HoraFin);
                cmd.Parameters.AddWithValue("@Lugar", model.Lugar);
                cmd.Parameters.AddWithValue("@CupoMaximo", model.CupoMaximo);
                cmd.Parameters.AddWithValue("@EstadoID", model.EstadoID);

                cn.Open();

                using var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (dr["Error"] != DBNull.Value)
                    {
                        TempData["Error"] = dr["Error"].ToString();
                        return View(model);
                    }

                    if (dr["Mensaje"] != DBNull.Value)
                    {
                        TempData["Exito"] = dr["Mensaje"].ToString();
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
            }

            return View(model);
        }
    }
}