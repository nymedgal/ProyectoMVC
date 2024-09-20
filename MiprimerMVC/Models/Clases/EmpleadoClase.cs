using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiprimerMVC.Models.Clases
{
    public class EmpleadoClase
    {

        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? ModificadoPor { get; set; }
    }
}