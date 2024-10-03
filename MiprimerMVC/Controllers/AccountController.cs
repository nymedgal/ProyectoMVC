using MiprimerMVC.Models;
using MiprimerMVC.Models.Clases;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MiprimerMVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(EmpleadoClase model)
        {
            if (ModelState.IsValid)
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var usuario = bd.MVC_Empleados
                                    .Where(u => u.Usuario == model.Usuario && u.Contrasena == model.Contrasena)
                                    .FirstOrDefault();
                    if (usuario != null)
                    {
                        FormsAuthentication.SetAuthCookie(usuario.Usuario, false);
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                }
            }
            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); // Cerrar sesión
            return RedirectToAction("Login", "Account"); // Redirigir al login después de cerrar sesión
        }
    }
}
