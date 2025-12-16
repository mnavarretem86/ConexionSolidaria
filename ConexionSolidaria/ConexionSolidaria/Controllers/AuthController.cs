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
            HttpContext.Session.SetInt32("UsuarioID", reader.GetInt32(0));
            HttpContext.Session.SetString("Nombre", reader.GetString(2));
            HttpContext.Session.SetString("Rol", reader.GetString(5));

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