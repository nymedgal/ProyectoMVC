using MiprimerMVC.Models.Clases;
using MiprimerMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MiprimerMVC.Controllers.Helpers;

namespace MiprimerMVC.Controllers
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        // GET: Empleado
        public ActionResult Index()
        {
            List<EmpleadoClase> listaEmpleados = null;
            using (var bd = new MVCPruebasEntities())
            {
                listaEmpleados =
                    (
                    from e in bd.MVC_Empleados
                    select new EmpleadoClase
                    {
                        IdEmpleado = e.IdEmpleado,
                        Nombre = e.Nombre,
                        Apellido = e.Apellido,
                        Usuario = e.Usuario,
                        Contrasena = e.Contrasena,
                        FechaCreacion = (DateTime)e.FechaCreacion,
                        FechaModificacion = (DateTime)e.FechaModificacion,
                        ModificadoPor = e.ModificadoPor
                    }
                    ).ToList();
            }
            Session["listaEmpleados"] = listaEmpleados;
            return View(listaEmpleados);
        }

        public ActionResult Editar(int id)
        {
            using (var bd = new MVCPruebasEntities())
            {
                var empleado = bd.MVC_Empleados.Where(c => c.IdEmpleado == id).FirstOrDefault();
                if (empleado != null)
                {
                    var empleadoClase = new EmpleadoClase
                    {
                        Nombre = empleado.Nombre,
                        Apellido = empleado.Apellido,
                        Usuario= empleado.Usuario,
                        Contrasena= empleado.Contrasena
                    };
                    return Json(empleadoClase, JsonRequestBehavior.AllowGet);
                }
            }
            return new HttpNotFoundResult();
        }

        public ActionResult VistaPreviaTabla()
        {
            var listaEmpleados = Session["listaEmpleados"] as List<EmpleadoClase>;
            return PartialView("_TablaEmpleados", listaEmpleados);
        }

        public ActionResult ExportarExcel()
        {
            var listaEmpleados = Session["listaEmpleados"] as List<CursoClase>;
            // Generar el archivo Excel utilizando la función reutilizable
            byte[] excelFile = Utils.ExportarAExcel(listaEmpleados);

            if (excelFile != null)
            {
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Cursos.xlsx");
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo generar el archivo Excel.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Agregar(EmpleadoClase empleadoClase)
        {
            if (ModelState.IsValid)
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var usuarioExistente = bd.MVC_Empleados
                        .FirstOrDefault(c => c.Nombre == empleadoClase.Usuario);

                    if (usuarioExistente != null)
                    {
                        TempData["ErrorMessage"] = "Ya existe un usuario con ese nombre.";
                        return RedirectToAction("Index");
                    }

                    try
                    {
                        var nuevoEmpleado = new MVC_Empleados
                        {
                            Nombre = empleadoClase.Usuario,
                            Apellido = empleadoClase.Apellido,
                            Usuario = empleadoClase.Usuario,
                            Contrasena = empleadoClase.Contrasena,
                            FechaCreacion = DateTime.Now,

                        };
                        bd.MVC_Empleados.Add(nuevoEmpleado);
                        bd.SaveChanges();

                        TempData["SuccessMessage"] = "Empleado agregado con exito.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Error al agregar el empleado: " + ex.Message;
                    }
                }
            }

            TempData["ErrorMessage"] = "Datos no válidos.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Editar(EmpleadoClase empleadoClase)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var bd = new MVCPruebasEntities())
                    {
                        var empladoExistente = bd.MVC_Empleados.Where(c => c.IdEmpleado == empleadoClase.IdEmpleado).FirstOrDefault();
                        if (empladoExistente != null)
                        {
                            empladoExistente.Nombre = empleadoClase.Nombre;
                            empladoExistente.Apellido = empleadoClase.Apellido;
                            empladoExistente.Usuario = empleadoClase.Usuario;
                            empladoExistente.Contrasena = empleadoClase.Contrasena;
                            empladoExistente.FechaModificacion = DateTime.Now;
                            // IMPLEMENTAR LOGICA DE ID LOGEADO
                            empladoExistente.ModificadoPor = 5;
                            bd.SaveChanges();
                            TempData["SuccessMessage"] = "Empleado editado con exito.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Error al editar el empleado";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Eliminar(int IdEmpleado)
        {
            try
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var empleado = bd.MVC_Empleados.FirstOrDefault(c => c.IdEmpleado == IdEmpleado);
                    if (empleado != null)
                    {
                        bd.MVC_Empleados.Remove(empleado);
                        bd.SaveChanges();
                        TempData["SuccessMessage"] = "Empleado eliminado con exito.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al eliminar el empleado";
            }

            return RedirectToAction("Index");
        }
    }
}