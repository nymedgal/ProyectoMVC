using MiprimerMVC.Models.Clases;
using MiprimerMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.IO;
using MiprimerMVC.Controllers.Helpers;

namespace MiprimerMVC.Controllers
{
    public class CursoController : Controller
    {
        // GET: Curso
        public ActionResult Index()
        {
            List<CursoClase> listaCursos = null;
            using (var bd = new MVCPruebasEntities())
            {
                listaCursos =
                    (
                    from c in bd.MVC_Cursos
                    select new CursoClase
                    {
                        IdCurso = c.IdCurso,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        FechaCreacion = (DateTime)c.FechaCreacion,
                        CreadoPor = c.CreadoPor,
                        FechaModificacion = (DateTime)c.FechaModificacion,
                        ModificadoPor = c.ModificadoPor
                    }
                    ).ToList();
            }
            Session["listaCursos"] = listaCursos;
            return View(listaCursos);
        }

        public ActionResult Editar(int id)
        {
            using (var bd = new MVCPruebasEntities())
            {
                var curso = bd.MVC_Cursos.Where(c => c.IdCurso == id).FirstOrDefault();
                if (curso != null)
                {
                    var cursoClase = new CursoClase
                    {
                        IdCurso = curso.IdCurso,
                        Nombre = curso.Nombre,
                        Descripcion = curso.Descripcion
                    };
                    return Json(cursoClase, JsonRequestBehavior.AllowGet);
                }
            }
            return new HttpNotFoundResult();
        }

        public ActionResult VistaPreviaTabla()
        {
            //using(var bd = new MVCPruebasEntities())
            //{
            //    var cursos = bd.MVC_Cursos.ToList();
            //    return PartialView("_TablaCursos", cursos);
            //}

            var listaCursos = Session["listaCursos"] as List<CursoClase>;
            return PartialView("_TablaCursos", listaCursos);

        }

        public ActionResult ExportarExcel()
        {
            var listaCursos = Session["listaCursos"] as List<CursoClase>;
            // Generar el archivo Excel utilizando la función reutilizable
            byte[] excelFile = Utils.ExportarAExcel(listaCursos);

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
        public ActionResult Agregar(CursoClase cursoClase)
        {
            if (ModelState.IsValid)
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var cursoExistente = bd.MVC_Cursos
                        .FirstOrDefault(c => c.Nombre == cursoClase.Nombre);

                    if (cursoExistente != null)
                    {
                        TempData["ErrorMessage"] = "Ya existe un curso con ese nombre.";
                        return RedirectToAction("Index");
                    }

                    try
                    {
                        var nuevoCurso = new MVC_Cursos
                        {
                            Nombre = cursoClase.Nombre,
                            Descripcion = cursoClase.Descripcion,
                            FechaCreacion = DateTime.Now,
                            CreadoPor = 5, // IMPLEMENTAR LOGICA DE ID LOGEADO
                        };
                        bd.MVC_Cursos.Add(nuevoCurso);
                        bd.SaveChanges();

                        TempData["SuccessMessage"] = "Curso agregado con exito.";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Error al agregar el curso: " + ex.Message;
                    }
                }
            }

            TempData["ErrorMessage"] = "Datos no válidos.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Editar(CursoClase cursoClase)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var bd = new MVCPruebasEntities())
                    {
                        var cursoExistente = bd.MVC_Cursos.Where(c => c.IdCurso == cursoClase.IdCurso).FirstOrDefault();
                        if (cursoExistente != null)
                        {
                            cursoExistente.Nombre = cursoClase.Nombre;
                            cursoExistente.Descripcion = cursoClase.Descripcion;
                            cursoExistente.FechaModificacion = DateTime.Now;
                            // IMPLEMENTAR LOGICA DE ID LOGEADO
                            cursoExistente.ModificadoPor = 5;
                            bd.SaveChanges();
                            TempData["SuccessMessage"] = "Curso editado con exito.";
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Error al editar el curso";
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Eliminar(int IdCurso)
        {
            try
            {
                using (var bd = new MVCPruebasEntities())
                {
                    var curso = bd.MVC_Cursos.FirstOrDefault(c => c.IdCurso == IdCurso);
                    if (curso != null)
                    {
                        bd.MVC_Cursos.Remove(curso);
                        bd.SaveChanges();
                        TempData["SuccessMessage"] = "Curso eliminado con exito.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error al eliminar el curso";
            }

            return RedirectToAction("Index");
        }

    }
}