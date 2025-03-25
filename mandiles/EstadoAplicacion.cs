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
        public Dictionary<string, List<string>> AsignacionesCajas { get; set; }
        public Dictionary<string, bool> EstadosCajas { get; set; }
        public Dictionary<string, bool> DescansosCajas { get; set; }
        public List<string> CajasEnDescanso { get; set; }
        public DateTime FechaHoraBackup { get; set; }
    }
}
