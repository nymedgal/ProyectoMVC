using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiprimerMVC.Models.Clases
{
    public class EmpleadosCursosClase { 
        public int IdEmpleado { get; set; } 
        public int IdCurso { get; set; } 
        public DateTime? FechaCreacion { get; set; } 
        public int? CreadoPor { get; set; } 
        public DateTime? FechaModificacion { get; set; } 
        public int? ModificadoPor { get; set; }

        //SE AGREGAN PARA LINQ CON INNER
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}