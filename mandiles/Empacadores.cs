using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mandiles
{
    // Clase Empacadores
    internal class Empacadores
    {
        private Dictionary<string, Dictionary<string, string>> horarioEmpacadores = new Dictionary<string, Dictionary<string, string>>();

        public Dictionary<string, Dictionary<string, string>> ObtenerHorario()
        {
            return horarioEmpacadores;
        }

        public Empacadores()
        {
            // Definir horarios de ejemplo al crear la instancia
            AsignarHorario("Demetrio Lozano", "Lunes", "6/10");
            AsignarHorario("Demetrio Lozano", "Martes", "6/10");
            AsignarHorario("Demetrio Lozano", "Miercoles", "6/10");
            AsignarHorario("Demetrio Lozano", "Jueves", "DESCANSO");
            AsignarHorario("Demetrio Lozano", "Viernes", "6/10");
            AsignarHorario("Demetrio Lozano", "Sabado", "3/10");
            AsignarHorario("Demetrio Lozano", "Domingo", "3/10");
            AsignarHorario("Juan Pérez", "Lunes", "9/11");
            AsignarHorario("María García", "Miércoles", "10/14");
        }

        public void AsignarHorario(string nombre, string dia, string horario)
        {
            if (!horarioEmpacadores.ContainsKey(nombre))
            {
                horarioEmpacadores[nombre] = new Dictionary<string, string>();
            }
            horarioEmpacadores[nombre][dia] = horario;
        }
    }
}
