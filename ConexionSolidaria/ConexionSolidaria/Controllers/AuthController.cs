using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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

            SqlCommand cmd = new SqlCommand("USP_LOGIN", _connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EMAIL", email);
            cmd.Parameters.AddWithValue("@CONTRASENA", contrasena);

            SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                return Json(new { success = false, message = "Credenciales incorrectas" });
            }

            reader.Read();

            int usuarioID = reader.GetInt32(0);

            int? voluntarioID = reader.IsDBNull(1)
                ? null
                : reader.GetInt32(1);

            string nombre = reader.GetString(2);
            string rol = reader.GetString(5);

            bool debeCambiarPassword = reader.GetBoolean(6);

            HttpContext.Session.SetInt32("UsuarioID", usuarioID);

            if (voluntarioID != null)
            {
                HttpContext.Session.SetInt32("VoluntarioID", voluntarioID.Value);
            }

            HttpContext.Session.SetString("Nombre", nombre);
            HttpContext.Session.SetString("Rol", rol);
            HttpContext.Session.SetInt32("DebeCambiarPassword", debeCambiarPassword ? 1 : 0);

            //Si tiene contraseña temporal
            if (debeCambiarPassword)
            {
                return Json(new { success = true, cambiarPassword = true });
            }

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


    [HttpGet]
    public IActionResult CambiarPassword()
    {
        if (HttpContext.Session.GetInt32("UsuarioID") == null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }


    [HttpPost]
    public IActionResult CambiarPassword(string nuevaPassword)
    {
        try
        {
            int usuarioID = HttpContext.Session.GetInt32("UsuarioID").Value;

            _connection.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE Usuario SET Contrasena=@pass, DebeCambiarPassword=0 WHERE UsuarioID=@id",
                _connection
            );

            cmd.Parameters.AddWithValue("@pass", nuevaPassword);
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