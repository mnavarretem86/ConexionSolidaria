using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ConexionSolidaria.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace ConexionSolidaria.Controllers
{
    public class RegistroController : Controller
    {
        private readonly IConfiguration _config;

        public RegistroController(IConfiguration config)
        {
            _config = config;
        }

        private string ConnectionString =>
            _config.GetConnectionString("DefaultConnection");


        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registrar(PersonaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join(" | ",
                    ModelState.Values
                              .SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage));

                return BadRequest(errores);
            }

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("USP_REGISTRO", cn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@PrimerNombre", model.PrimerNombre);
            cmd.Parameters.AddWithValue("@SegundoNombre", (object?)model.SegundoNombre ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PrimerApellido", model.PrimerApellido);
            cmd.Parameters.AddWithValue("@SegundoApellido", (object?)model.SegundoApellido ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DNI", model.DNI);
            cmd.Parameters.AddWithValue("@Genero", model.Genero);
            cmd.Parameters.AddWithValue("@FechaNacimiento", (object?)model.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@Telefono", (object?)model.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)model.Direccion ?? DBNull.Value);

            cn.Open();

            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                if (TieneColumna(dr, "Error"))
                    return BadRequest(dr["Error"].ToString());

                if (TieneColumna(dr, "Mensaje"))
                {
                    var mensaje = dr["Mensaje"].ToString();

                    if (TieneColumna(dr, "PasswordTemporal"))
                    {
                        mensaje += $" | Contraseña temporal: {dr["PasswordTemporal"]}";
                    }

                    return Ok(mensaje);
                }
            }

            return Ok("Registro exitoso");
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