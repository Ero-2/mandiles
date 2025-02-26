using System;
using System.Collections.Generic;
using System.Globalization;
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

       

        public void AsignarHorario(string nombre, string dia, string horario)
        {
            if (!horarioEmpacadores.ContainsKey(nombre))
            {
                horarioEmpacadores[nombre] = new Dictionary<string, string>();
            }
            horarioEmpacadores[nombre][dia] = horario;
        }

        public List<string> ObtenerEmpacadoresDelHorario(DateTime fecha)
        {
            string diaSemana = fecha.ToString("dddd", new CultureInfo("es-ES"));
            // Si en tus asignaciones usas "Miercoles" sin tilde, conviértelo:
            if (diaSemana.Equals("miércoles", StringComparison.OrdinalIgnoreCase))
            {
                diaSemana = "Miercoles";
            }

            List<string> empacadoresDelDia = new List<string>();
            foreach (var emp in horarioEmpacadores)
            {
                if (emp.Value.ContainsKey(diaSemana) &&
                    !string.IsNullOrEmpty(emp.Value[diaSemana]) &&
                    emp.Value[diaSemana].ToUpper() != "DESCANSO")
                {
                    empacadoresDelDia.Add(emp.Key);
                }
            }
            return empacadoresDelDia;
        }
    }
}

