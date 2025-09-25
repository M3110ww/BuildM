using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildM.Models;

public class Material
{
    public int IdMaterial { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty; 
    public int Stock { get; set; }
    public decimal Costo { get; set; }
}
