using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.Data;
using BCrypt.Net;

public class AuthController : Controller
{
    private readonly SqlConnection _connection;

    public AuthController(SqlConnection connection)
    {
        _connection = connection;
    }

    [HttpPost]
    public IActionResult Login(string email, string contrasena)
    {
        try
        {
            _connection.Open();

            using SqlCommand cmd = new("USP_LOGIN", _connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EMAIL", email);

            using SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.Read())
                return Json(new { success = false, message = "Credenciales incorrectas" });

            int usuarioID = reader.GetInt32(0);
            int? voluntarioID = reader.IsDBNull(1) ? null : reader.GetInt32(1);

            string nombre = reader.GetString(2) + " " + reader.GetString(3);
            string hash = reader.GetString(4);
            string rol = reader.GetString(6);
            bool debeCambiarPassword = reader.GetBoolean(7);

            if (!BCrypt.Net.BCrypt.Verify(contrasena, hash))
                return Json(new { success = false, message = "Credenciales incorrectas" });

            HttpContext.Session.SetInt32("UsuarioID", usuarioID);

            if (voluntarioID.HasValue)
                HttpContext.Session.SetInt32("VoluntarioID", voluntarioID.Value);

            HttpContext.Session.SetString("Nombre", nombre);
            HttpContext.Session.SetString("Rol", rol);
            HttpContext.Session.SetInt32("DebeCambiarPassword", debeCambiarPassword ? 1 : 0);

            return Json(new { success = true, cambiarPassword = debeCambiarPassword });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        finally
        {
            _connection.Close();
        }
    }

    [HttpGet]
    public IActionResult CambiarPassword()
    {
        if (HttpContext.Session.GetInt32("UsuarioID") == null)
            return RedirectToAction("Index", "Home");

        bool esTemporal = HttpContext.Session.GetInt32("DebeCambiarPassword") == 1;

        ViewBag.EsTemporal = esTemporal;

        return View();
    }

    [HttpPost]
    public IActionResult CambiarPassword(string nuevaPassword, string passwordActual)
    {
        try
        {
            int usuarioID = HttpContext.Session.GetInt32("UsuarioID") ?? 0;
            bool esTemporal = HttpContext.Session.GetInt32("DebeCambiarPassword") == 1;

            _connection.Open();

            if (!esTemporal)
            {
                using SqlCommand cmdCheck = new(
                    "SELECT Contrasena FROM Usuario WHERE UsuarioID=@id",
                    _connection
                );

                cmdCheck.Parameters.AddWithValue("@id", usuarioID);

                string hashActual = cmdCheck.ExecuteScalar()?.ToString();

                if (!BCrypt.Net.BCrypt.Verify(passwordActual, hashActual))
                    return Json(new { success = false, message = "Contraseña actual incorrecta" });
            }

            string hashNueva = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);

            using SqlCommand cmd = new(
                "UPDATE Usuario SET Contrasena=@pass, DebeCambiarPassword=0 WHERE UsuarioID=@id",
                _connection
            );

            cmd.Parameters.AddWithValue("@pass", hashNueva);
            cmd.Parameters.AddWithValue("@id", usuarioID);

            cmd.ExecuteNonQuery();

            HttpContext.Session.SetInt32("DebeCambiarPassword", 0);

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
        finally
        {
            _connection.Close();
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}