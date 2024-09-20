using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiprimerMVC.Models.Clases
{
    public class CursoClase
    {
        public int IdCurso { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? CreadoPor { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? ModificadoPor { get; set; }
    }
}
