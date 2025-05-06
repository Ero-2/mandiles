using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandiles
{
    [Serializable]
    internal class EstadoAplicacion
    {
        
            public Dictionary<string, List<string>> Asignaciones { get; set; } = new Dictionary<string, List<string>>();
            public Dictionary<string, bool> CajasAbiertas { get; set; } = new Dictionary<string, bool>();
            public Dictionary<string, bool> CajasEnDescanso { get; set; } = new Dictionary<string, bool>();
            public List<string> EmpacadoresEnEspera { get; set; } = new List<string>();
            public Dictionary<string, List<string>> CierresTemporales { get; set; } = new Dictionary<string, List<string>>();
        
    }
}
