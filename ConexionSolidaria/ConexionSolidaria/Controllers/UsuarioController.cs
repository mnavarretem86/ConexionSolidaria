using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;
using ConexionSolidaria.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace ConexionSolidaria.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IConfiguration _config;

        public UsuarioController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString =>
            _config.GetConnectionString("DefaultConnection");

        private bool EsCoordinador()
        {
            return HttpContext.Session.GetString("Rol") == "Coordinador";
        }

        /* =====================================================
           LISTADO
        ===================================================== */
        public IActionResult Index()
        {
            if (!EsCoordinador())
                return RedirectToAction("Index", "Dashboard");

            var lista = new List<UsuarioViewModel>();

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_USUARIO", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Opcion", 3);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new UsuarioViewModel
                {
                    UsuarioID = (int)dr["UsuarioID"],
                    PersonaID = (int)dr["PersonaID"],
                    NombreCompleto = dr["NombreCompleto"].ToString(),
                    Email = dr["Email"] as string,
                    RolID = dr["RolID"] == DBNull.Value ? 0 : (int)dr["RolID"],
                    NombreRol = dr["NombreRol"] as string,
                    EstadoID = (int)dr["EstadoID"]
                });
            }

            return View(lista);
        }

        /* =====================================================
           PERSONAS SIN USUARIO (SP)
        ===================================================== */
        [HttpGet]
        public IActionResult PersonasSinUsuario()
        {
            if (!EsCoordinador())
                return Unauthorized();

            var personas = new List<object>();

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_PERSONAS_SIN_USUARIO", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                personas.Add(new
                {
                    PersonaID = (int)dr["PersonaID"],
                    NombreCompleto = dr["NombreCompleto"].ToString(),
                    Email = dr["Email"] as string
                });
            }

            return Json(personas);
        }

        /* =====================================================
           CREAR
        ===================================================== */
        [HttpPost]
        public IActionResult Crear(UsuarioViewModel model)
        {
            if (!EsCoordinador())
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest("Datos inválidos");

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_USUARIO", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Opcion", 1);
            cmd.Parameters.AddWithValue("@PersonaID", model.PersonaID);
            cmd.Parameters.AddWithValue("@Contrasena", model.Contrasena);
            cmd.Parameters.AddWithValue("@RolID", model.RolID);
            cmd.Parameters.AddWithValue("@EstadoID", model.EstadoID);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                if (TieneColumna(dr, "Error"))
                    return BadRequest(dr["Error"].ToString());

                if (TieneColumna(dr, "Mensaje"))
                    return Ok(dr["Mensaje"].ToString());
            }

            return Ok("Usuario creado correctamente");
        }

        /* =====================================================
           EDITAR
        ===================================================== */
        [HttpPost]
        public IActionResult Editar(UsuarioViewModel model)
        {
            if (!EsCoordinador())
                return Unauthorized();

            if (model.UsuarioID <= 0)
                return BadRequest("UsuarioID inválido");

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_USUARIO", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Opcion", 2);
            cmd.Parameters.AddWithValue("@UsuarioID", model.UsuarioID);
            cmd.Parameters.AddWithValue("@Contrasena", model.Contrasena);
            cmd.Parameters.AddWithValue("@RolID", model.RolID);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                if (TieneColumna(dr, "Error"))
                    return BadRequest(dr["Error"].ToString());

                if (TieneColumna(dr, "Mensaje"))
                    return Ok(dr["Mensaje"].ToString());
            }

            return Ok("Usuario actualizado correctamente");
        }

        /* =====================================================
           ROLES
        ===================================================== */
        [HttpGet]
        public IActionResult ObtenerRoles()
        {
            if (!EsCoordinador())
                return Unauthorized();

            var roles = new List<object>();

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand(
                "SELECT RolID, NombreRol FROM Rol ORDER BY NombreRol",
                cn
            );

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                roles.Add(new
                {
                    RolID = (int)dr["RolID"],
                    NombreRol = dr["NombreRol"].ToString()
                });
            }

            return Json(roles);
        }

        /* =====================================================
           UTILIDAD
        ===================================================== */
        private bool TieneColumna(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i)
                          .Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
