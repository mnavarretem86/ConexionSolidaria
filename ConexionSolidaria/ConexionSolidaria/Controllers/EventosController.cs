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

        private bool EsCoordinador()
        {
            return HttpContext.Session.GetString("Rol") == "Coordinador";
        }

        private int? UsuarioID()
        {
            return HttpContext.Session.GetInt32("UsuarioID");
        }

        private int? VoluntarioID()
        {
            return HttpContext.Session.GetInt32("VoluntarioID");
        }

        public IActionResult Index()
        {
            if (UsuarioID() == null)
                return RedirectToAction("Index", "Home");

            var lista = new List<EventoListadoVM>();

            try
            {
                using var cn = new SqlConnection(ConnectionString);
                using var cmd = new SqlCommand("USP_EVENTO", cn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Opcion", 3);

                if (EsCoordinador())
                    cmd.Parameters.AddWithValue("@VoluntarioID", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@VoluntarioID", (object?)VoluntarioID() ?? DBNull.Value);

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
            if (!EsCoordinador())
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Evento model)
        {
            if (!EsCoordinador())
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            var usuarioId = UsuarioID();

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

                string mensaje = ObtenerMensaje(cmd);

                if (mensaje.Contains("Error"))
                {
                    TempData["Error"] = mensaje;
                    return View(model);
                }

                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo crear el evento: " + ex.Message;
            }

            return View(model);
        }

        public IActionResult Editar(int id)
        {
            if (!EsCoordinador())
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
                TempData["Error"] = ex.Message;
            }

            if (evento == null)
                return NotFound();

            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Evento model)
        {
            if (!EsCoordinador())
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

                string mensaje = ObtenerMensaje(cmd);

                if (mensaje.Contains("Error"))
                {
                    TempData["Error"] = mensaje;
                    return View(model);
                }

                TempData["Exito"] = mensaje;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar: " + ex.Message;
            }

            return View(model);
        }

        private string ObtenerMensaje(SqlCommand cmd)
        {
            using var dr = cmd.ExecuteReader();

            if (dr.Read())
                return dr[0].ToString();

            return "Operación completada";
        }
    }
}