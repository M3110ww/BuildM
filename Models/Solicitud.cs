using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildM.Models
{
    public class Solicitud
    {
        public int IdSolicitud { get; set; }
        public string NombreProyecto { get; set; }
        public string Responsable { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
    }
}
