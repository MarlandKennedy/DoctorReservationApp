using DoctorReservationApp.Models;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Configuration;
using System.Data;

namespace DoctorReservationApp.Controllers
{
    public class AccountController : Controller
    {

        private readonly MySqlConnection _connection;

    public AccountController()
        {
            //foreach (ConnectionStringSettings connection in System.Configuration.ConfigurationManager.ConnectionStrings) { }
                //_connection = new MySqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString);
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
           
            if (ModelState.IsValid)
                {
                    // Authenticate the user and redirect to the home page
                    return RedirectToAction("Index", "Home");
                }
            int result = 0;

            using (var command = new MySqlCommand("ValidateUserLogin", _connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@p_username", model.Email);
                command.Parameters.AddWithValue("@p_password", model.Password);
                command.Parameters.Add("@p_result", MySqlDbType.Int32).Direction = ParameterDirection.Output;

                _connection.Open();
                command.ExecuteNonQuery();
                _connection.Close();

                result = Convert.ToInt32(command.Parameters["@p_result"].Value);
            }

            if (result == 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }
    }
}
