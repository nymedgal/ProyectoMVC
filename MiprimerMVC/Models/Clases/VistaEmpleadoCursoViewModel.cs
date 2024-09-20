using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiprimerMVC.Models.Clases
{
    public class VistaEmpleadoCursoViewModel
    {
        public string Nombre { get; set; }
        public string CursoDescripcion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? CreadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? ModificadoPor { get; set; }
    }

}