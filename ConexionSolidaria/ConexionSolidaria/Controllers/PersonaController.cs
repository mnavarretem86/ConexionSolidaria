using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;
using ConexionSolidaria.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ConexionSolidaria.Controllers
{
    public class PersonaController : Controller
    {
        private readonly IConfiguration _config;

        public PersonaController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString =>
            _config.GetConnectionString("DefaultConnection");

        private bool EsCoordinador()
        {
            return HttpContext.Session.GetString("Rol") == "Coordinador";
        }
        public IActionResult Index()
        {
            if (!EsCoordinador())
                return RedirectToAction("Index", "Dashboard");

            var lista = new List<PersonaViewModel>();

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_PERSONA", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Opcion", 3);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new PersonaViewModel
                {
                    PersonaID = (int)dr["PersonaID"],
                    NombreCompleto = dr["NombreCompleto"].ToString(),
                    PrimerNombre = dr["PrimerNombre"].ToString(),
                    SegundoNombre = dr["SegundoNombre"] as string,
                    PrimerApellido = dr["PrimerApellido"].ToString(),
                    SegundoApellido = dr["SegundoApellido"] as string,
                    DNI = dr["DNI"].ToString(),
                    Genero = dr["Genero"].ToString()[0],
                    FechaNacimiento = dr["FechaNacimiento"] as DateTime?,
                    Edad = (int)dr["Edad"],
                    Email = dr["Email"] as string,
                    Telefono = dr["Telefono"] as string,
                    Direccion = dr["Direccion"] as string
                });
            }

            return View(lista);
        }

        [HttpPost]
        public IActionResult Crear(PersonaViewModel model)
        {
            if (!EsCoordinador())
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values
                              .SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage));

                return BadRequest(errores);
            }

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_PERSONA", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Opcion", 1);
            cmd.Parameters.AddWithValue("@PrimerNombre", model.PrimerNombre);
            cmd.Parameters.AddWithValue("@SegundoNombre", (object?)model.SegundoNombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrimerApellido", model.PrimerApellido);
            cmd.Parameters.AddWithValue("@SegundoApellido", (object?)model.SegundoApellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DNI", model.DNI);
            cmd.Parameters.AddWithValue("@Genero", model.Genero);
            cmd.Parameters.AddWithValue("@FechaNacimiento", (object?)model.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)model.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)model.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)model.Direccion ?? DBNull.Value);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                if (TieneColumna(dr, "Error"))
                    return BadRequest(dr["Error"].ToString());

                if (TieneColumna(dr, "Mensaje"))
                    return Ok(dr["Mensaje"].ToString());
            }

            return Ok("Persona creada correctamente");
        }
        [HttpPost]
        public IActionResult Editar(PersonaViewModel model)
        {
            if (!EsCoordinador())
                return Unauthorized();

            if (model.PersonaID <= 0)
                return BadRequest("PersonaID inválido");

            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values
                              .SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage));

                return BadRequest(errores);
            }

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_PERSONA", cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Opcion", 2);
            cmd.Parameters.AddWithValue("@PersonaID", model.PersonaID);
            cmd.Parameters.AddWithValue("@PrimerNombre", model.PrimerNombre);
            cmd.Parameters.AddWithValue("@SegundoNombre", (object?)model.SegundoNombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrimerApellido", model.PrimerApellido);
            cmd.Parameters.AddWithValue("@SegundoApellido", (object?)model.SegundoApellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DNI", model.DNI);
            cmd.Parameters.AddWithValue("@Genero", model.Genero);
            cmd.Parameters.AddWithValue("@FechaNacimiento", (object?)model.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", (object?)model.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Telefono", (object?)model.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)model.Direccion ?? DBNull.Value);

            cn.Open();
            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                if (TieneColumna(dr, "Error"))
                    return BadRequest(dr["Error"].ToString());

                if (TieneColumna(dr, "Mensaje"))
                    return Ok(dr["Mensaje"].ToString());
            }

            return Ok("Persona actualizada correctamente");
        }
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
