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
    public class EmpleadoCursoController : Controller
    {
        // GET: EmpleadoCurso
        public ActionResult Index()
        {
            List<EmpleadosCursosClase> listaEmpleadoCursos = null;
            List<MVC_Cursos> listaCursos = null;
            List<MVC_Empleados> listaEmpleados = null;

            using (var bd = new MVCPruebasEntities())
            {
                listaEmpleadoCursos =
                (
                    from ec in bd.MVC_EmpleadosCursos
                    join e in bd.MVC_Empleados on ec.IdEmpleado equals e.IdEmpleado
                    join c in bd.MVC_Cursos on ec.IdCurso equals c.IdCurso
                    select new EmpleadosCursosClase
                    {
                        IdEmpleado = ec.IdEmpleado,
                        IdCurso = ec.IdCurso,
                        NombreCompleto = string.Concat(e.Nombre, " ", e.Apellido), 
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        FechaCreacion = (DateTime)c.FechaCreacion,
                        CreadoPor = c.CreadoPor,
                        FechaModificacion = (DateTime)c.FechaModificacion,
                        ModificadoPor = c.ModificadoPor
                    }
                ).ToList();

                listaCursos = bd.MVC_Cursos.ToList();
                listaEmpleados = bd.MVC_Empleados.ToList();
            }
            Session["listaCursos"] = listaCursos;
            Session["listaEmpleados"] = listaEmpleados;
            Session["listaEmpleadoCursos"] = listaEmpleadoCursos;

            return View(listaEmpleadoCursos);
        }
        public ActionResult VistaPreviaTabla()
        {
            var listaCursos = Session["listaCursos"] as List<MVC_Cursos>;
            var listaEmpleados = Session["listaEmpleados"] as List<MVC_Empleados>;
            var listaEmpleadoCursos = Session["listaEmpleadoCursos"] as List<EmpleadosCursosClase>;

            if (listaCursos == null || listaEmpleados == null || listaEmpleadoCursos == null)
            {
                return HttpNotFound();
            }

            var vistaPrevia = from ec in listaEmpleadoCursos
                              join e in listaEmpleados on ec.IdEmpleado equals e.IdEmpleado
                              join c in listaCursos on ec.IdCurso equals c.IdCurso
                              select new VistaEmpleadoCursoViewModel
                              {
                                  Nombre = $"{e.Nombre} {e.Apellido}",
                                  CursoDescripcion = c.Descripcion,
                                  FechaCreacion = ec.FechaCreacion,
                                  CreadoPor = ec.CreadoPor,
                                  FechaModificacion = ec.FechaModificacion,
                                  ModificadoPor = ec.ModificadoPor
                              };
            Session["vistaPrevia"] = vistaPrevia.ToList();

            return PartialView("_TablaEmpleadoCurso", vistaPrevia.ToList()); // Asegúrate de llamar a ToList()
        }
        public ActionResult ExportarExcel()
        {
            var vistaPrevia = Session["vistaPrevia"] as List<VistaEmpleadoCursoViewModel>;
            // Generar el archivo Excel utilizando la función reutilizable
            byte[] excelFile = Utils.ExportarAExcel(vistaPrevia);

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
        public ActionResult ExportarExcelSeleccionados(string idsE, string idsC)
        {
            if (string.IsNullOrEmpty(idsE))
            {
                TempData["ErrorMessage"] = "No se ha seleccionado ningún empleado.";
                return RedirectToAction("Index");
            }

            var empleadoIds = idsE.Split(',').Select(int.Parse).ToList();
            var cursoIds = string.IsNullOrEmpty(idsC) ? new List<int>() : idsC.Split(',').Select(int.Parse).ToList();
            var listaEmpleadoCursos = Session["listaEmpleadoCursos"] as List<EmpleadosCursosClase>;

            if (listaEmpleadoCursos == null)
            {
                TempData["ErrorMessage"] = "No se pudo recuperar la lista de la sesión.";
                return RedirectToAction("Index");
            }

            // Filtrar los registros para obtener solo los cursos de los empleados seleccionados y sus respectivos cursos
            var empleadosSeleccionados = new List<EmpleadosCursosClase>();

            for (int i = 0; i < empleadoIds.Count; i++)
            {
                int empleadoId = empleadoIds[i];
                int cursoId = cursoIds[i];

                var ec = listaEmpleadoCursos.FirstOrDefault(x => x.IdEmpleado == empleadoId && x.IdCurso == cursoId);

                if (ec != null)
                {
                    empleadosSeleccionados.Add(ec);
                }
            }


            if (empleadosSeleccionados.Count == 0)
            {
                TempData["ErrorMessage"] = "No se encontraron cursos para los empleados seleccionados.";
                return RedirectToAction("Index");
            }
            byte[] excelFile = Utils.ExportarAExcel(empleadosSeleccionados);

            if (excelFile != null)
            {
                //TempData["SuccessMessage"] = "Descargando Excel...";
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EmpleadosSeleccionados.xlsx");
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo generar el archivo Excel.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Agregar(EmpleadosCursosClase ecCurso)
        {
            if (ModelState.IsValid)
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var usuarioCurso = bd.MVC_EmpleadosCursos
                        .FirstOrDefault(c => c.IdEmpleado == ecCurso.IdEmpleado && c.IdCurso == ecCurso.IdCurso);

                    if (usuarioCurso != null)
                    {
                        TempData["ErrorMessage"] = "Ya existe este empleado con ese curso.";
                        return RedirectToAction("Index");
                    }

                    try
                    {
                        var nuevoEC = new MVC_EmpleadosCursos
                        {
                            IdEmpleado = ecCurso.IdEmpleado,
                            IdCurso = ecCurso.IdCurso,
                            FechaCreacion = DateTime.Now,

                        };
                        bd.MVC_EmpleadosCursos.Add(nuevoEC);
                        bd.SaveChanges();

                        TempData["SuccessMessage"] = "Empleado se le asigno el curso con exito.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Error al agregar al asignar el curso al empleado: " + ex.Message;
                    }
                }
            }

            TempData["ErrorMessage"] = "Datos no válidos.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Eliminar(int IdEmpleado, int IdCurso)
        {
            try
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var empleadoCurso = bd.MVC_EmpleadosCursos
                                 .FirstOrDefault(ec => ec.IdEmpleado == IdEmpleado && ec.IdCurso == IdCurso);

                    if (empleadoCurso != null)
                    {
                        bd.MVC_EmpleadosCursos.Remove(empleadoCurso);
                        bd.SaveChanges();
                        TempData["SuccessMessage"] = "Se elimino el empleado del curso con exito.";
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

        [HttpPost]
        public ActionResult EliminarMasivamente(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                TempData["ErrorMessage"] = "No se han seleccionado empleados para eliminar.";
                return RedirectToAction("Index");
            }

            var empleadosIds = ids.Split(',').Select(int.Parse).ToList();
            var listaEmpleadoCursos = Session["listaEmpleadoCursos"] as List<EmpleadosCursosClase>;

            if (listaEmpleadoCursos == null)
            {
                TempData["ErrorMessage"] = "No se pudo recuperar la lista de la sesión.";
                return RedirectToAction("Index");
            }

            using (var bd = new MVCPruebasEntities())
            {
                foreach (var empleadoId in empleadosIds)
                {
                    var empleadoCursoEnSesion = listaEmpleadoCursos.FirstOrDefault(ec => ec.IdEmpleado == empleadoId);

                    if (empleadoCursoEnSesion != null)
                    {
                        var empleadoCursoEnBD = bd.MVC_EmpleadosCursos
                            .FirstOrDefault(ec => ec.IdEmpleado == empleadoCursoEnSesion.IdEmpleado && ec.IdCurso == empleadoCursoEnSesion.IdCurso);

                        if (empleadoCursoEnBD != null)
                        {
                            bd.MVC_EmpleadosCursos.Remove(empleadoCursoEnBD);
                        }
                    }
                }

                try
                {
                    bd.SaveChanges();
                    TempData["SuccessMessage"] = "Empleados eliminados exitosamente.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error al eliminar los empleados: " + ex.Message;
                }
            }

            return RedirectToAction("Index");
        }


    }
}