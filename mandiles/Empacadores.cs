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

        public Empacadores()
        {
            // DEMETRIO
            AsignarHorario("Demetrio Lozano", "Lunes", "6/10");
            AsignarHorario("Demetrio Lozano", "Martes", "6/10");
            AsignarHorario("Demetrio Lozano", "Miercoles", "6/10");
            AsignarHorario("Demetrio Lozano", "Jueves", "DESCANSO");
            AsignarHorario("Demetrio Lozano", "Viernes", "6/10");
            AsignarHorario("Demetrio Lozano", "Sabado", "3/10");
            AsignarHorario("Demetrio Lozano", "Domingo", "3/10");

            // DANIEL 
            AsignarHorario("Daniel Saucedo", "Lunes", "6/10");
            AsignarHorario("Daniel Saucedo", "Martes", "6/10");
            AsignarHorario("Daniel Saucedo", "Miercoles", "DESCANSO");
            AsignarHorario("Daniel Saucedo", "Jueves", "DESCANSO");
            AsignarHorario("Daniel Saucedo", "Viernes", "6/10");
            AsignarHorario("Daniel Saucedo", "Sabado", "3/10");
            AsignarHorario("Daniel Saucedo", "Domingo", "3/10");

            // FERNANDO
            AsignarHorario("Fernando Castellanos", "Lunes", "DESCANSO");
            AsignarHorario("Fernando Castellanos", "Martes", "6/10");
            AsignarHorario("Fernando Castellanos", "Miercoles", "6/10");
            AsignarHorario("Fernando Castellanos", "Jueves", "6/10");
            AsignarHorario("Fernando Castellanos", "Viernes", "6/10");
            AsignarHorario("Fernando Castellanos", "Sabado", "3/10");
            AsignarHorario("Fernando Castellanos", "Domingo", "3/10");

            // RENE DE LUNA
            AsignarHorario("Rene de Luna", "Lunes", "6/10");
            AsignarHorario("Rene de Luna", "Martes", "6/10");
            AsignarHorario("Rene de Luna", "Miercoles", "DESCANSO");
            AsignarHorario("Rene de Luna", "Jueves", "DESCANSO");
            AsignarHorario("Rene de Luna", "Viernes", "DESCANSO");
            AsignarHorario("Rene de Luna", "Sabado", "3/10");
            AsignarHorario("Rene de Luna", "Domingo", "3/10");

            // YAEL 
            AsignarHorario("Yael Chikilin", "Lunes", "3/7");
            AsignarHorario("Yael Chikilin", "Martes", "3/7");
            AsignarHorario("Yael Chikilin", "Miercoles", "3/7");
            AsignarHorario("Yael Chikilin", "Jueves", "3/7");
            AsignarHorario("Yael Chikilin", "Viernes", "3/7");
            AsignarHorario("Yael Chikilin", "Sabado", "DESCANSO");
            AsignarHorario("Yael Chikilin", "Domingo", "3/10");

            // PEDRO ALVARADO
            AsignarHorario("Pedro Alvarado", "Lunes", "3/10");
            AsignarHorario("Pedro Alvarado", "Martes", "3/10");
            AsignarHorario("Pedro Alvarado", "Miercoles", "3/10");
            AsignarHorario("Pedro Alvarado", "Jueves", "DESCANSO");
            AsignarHorario("Pedro Alvarado", "Viernes", "3/10");
            AsignarHorario("Pedro Alvarado", "Sabado", "3/10");
            AsignarHorario("Pedro Alvarado", "Domingo", "3/10");

            // JOSE CHIKILIN
            AsignarHorario("Jose Chikilin", "Lunes", "DESCANSO");
            AsignarHorario("Jose Chikilin", "Martes", "DESCANSO");
            AsignarHorario("Jose Chikilin", "Miercoles", "DESCANSO");
            AsignarHorario("Jose Chikilin", "Jueves", "DESCANSO");
            AsignarHorario("Jose Chikilin", "Viernes", "6/10");
            AsignarHorario("Jose Chikilin", "Sabado", "3/10");
            AsignarHorario("Jose Chikilin", "Domingo", "3/10");
            // RICARDO OCHOA
            AsignarHorario("Ricardo Ochoa", "Lunes", "6/10");
            AsignarHorario("Ricardo Ochoa", "Martes", "6/10");
            AsignarHorario("Ricardo Ochoa", "Miercoles", "DESCANSO");
            AsignarHorario("Ricardo Ochoa", "Jueves", "DESCANSO");
            AsignarHorario("Ricardo Ochoa", "Viernes", "DESCANSO");
            AsignarHorario("Ricardo Ochoa", "Sabado", "3/10");
            AsignarHorario("Ricardo Ochoa", "Domingo", "3/10");
            // JOSE ROJAS
            AsignarHorario("Jose Rojas", "Lunes", "3/10");
            AsignarHorario("Jose Rojas", "Martes", "DESCANSO");
            AsignarHorario("Jose Rojas", "Miercoles", "3/10");
            AsignarHorario("Jose Rojas", "Jueves", "3/10");
            AsignarHorario("Jose Rojas", "Viernes", "3/10");
            AsignarHorario("Jose Rojas", "Sabado", "3/10");
            AsignarHorario("Jose Rojas", "Domingo", "3/10");
            // DANIEL OCHOA
            AsignarHorario("Daniel Ochoa", "Lunes", "DESCANSO");
            AsignarHorario("Daniel Ochoa", "Martes", "DESCANSO");
            AsignarHorario("Daniel Ochoa", "Miercoles", "DESCANSO");
            AsignarHorario("Daniel Ochoa", "Jueves", "DESCANSO");
            AsignarHorario("Daniel Ochoa", "Viernes", "6/10");
            AsignarHorario("Daniel Ochoa", "Sabado", "3/10");
            AsignarHorario("Daniel Ochoa", "Domingo", "3/10");
            // ANGELY VARGAS
            AsignarHorario("Angely Vargas", "Lunes", "6/10");
            AsignarHorario("Angely Vargas", "Martes", "6/10");
            AsignarHorario("Angely Vargas", "Miercoles", "DESCANSO");
            AsignarHorario("Angely Vargas", "Jueves", "6/10");
            AsignarHorario("Angely Vargas", "Viernes", "3/10");
            AsignarHorario("Angely Vargas", "Sabado", "3/10");
            AsignarHorario("Angely Vargas", "Domingo", "3/10");
            // NELLY VARGAS
            AsignarHorario("Nelly Vargas", "Lunes", "DESCANSO");
            AsignarHorario("Nelly Vargas", "Martes", "6/10");
            AsignarHorario("Nelly Vargas", "Miercoles", "6/10");
            AsignarHorario("Nelly Vargas", "Jueves", "6/10");
            AsignarHorario("Nelly Vargas", "Viernes", "3/10");
            AsignarHorario("Nelly Vargas", "Sabado", "3/10");
            AsignarHorario("Nelly Vargas", "Domingo", "3/10");
            // EVELYN PEREZ
            AsignarHorario("Evelyn Perez", "Lunes", "6/10");
            AsignarHorario("Evelyn Perez", "Martes", "DESCANSO");
            AsignarHorario("Evelyn Perez", "Miercoles", "DESCANSO");
            AsignarHorario("Evelyn Perez", "Jueves", "6/10");
            AsignarHorario("Evelyn Perez", "Viernes", "3/10");
            AsignarHorario("Evelyn Perez", "Sabado", "3/10");
            AsignarHorario("Evelyn Perez", "Domingo", "3/10");
            // YESS
            AsignarHorario("Yess", "Lunes", "DESCANSO");
            AsignarHorario("Yess", "Martes", "DESCANSO");
            AsignarHorario("Yess", "Miercoles", "DESCANSO");
            AsignarHorario("Yess", "Jueves", "DESCANSO");
            AsignarHorario("Yess", "Viernes", "6/10");
            AsignarHorario("Yess", "Sabado", "3/10");
            AsignarHorario("Yess", "Domingo", "3/10");
            // MARY
            AsignarHorario("Mary", "Lunes", "DESCANSO");
            AsignarHorario("Mary", "Martes", "DESCANSO");
            AsignarHorario("Mary", "Miercoles", "DESCANSO");
            AsignarHorario("Mary", "Jueves", "DESCANSO");
            AsignarHorario("Mary", "Viernes", "6/10");
            AsignarHorario("Mary", "Sabado", "3/10");
            AsignarHorario("Mary", "Domingo", "3/10");
            // LUZ RUELAS
            AsignarHorario("Luz Ruelas", "Lunes", "3/10");
            AsignarHorario("Luz Ruelas", "Martes", "3/10");
            AsignarHorario("Luz Ruelas", "Miercoles", "3/10");
            AsignarHorario("Luz Ruelas", "Jueves", "3/10");
            AsignarHorario("Luz Ruelas", "Viernes", "DESCANSO");
            AsignarHorario("Luz Ruelas", "Sabado", "3/10");
            AsignarHorario("Luz Ruelas", "Domingo", "3/10");
            // VICKY FRAUSTO
            AsignarHorario("Vicky Frausto", "Lunes", "3/10");
            AsignarHorario("Vicky Frausto", "Martes", "DESCANSO");
            AsignarHorario("Vicky Frausto", "Miercoles", "3/10");
            AsignarHorario("Vicky Frausto", "Jueves", "3/10");
            AsignarHorario("Vicky Frausto", "Viernes", "3/10");
            AsignarHorario("Vicky Frausto", "Sabado", "3/10");
            AsignarHorario("Vicky Frausto", "Domingo", "3/10");
            // MARIA SANTILLANA
            AsignarHorario("Maria Santillana", "Lunes", "3/10");
            AsignarHorario("Maria Santillana", "Martes", "3/10");
            AsignarHorario("Maria Santillana", "Miercoles", "3/10");
            AsignarHorario("Maria Santillana", "Jueves", "3/10");
            AsignarHorario("Maria Santillana", "Viernes", "DESCANSO");
            AsignarHorario("Maria Santillana", "Sabado", "3/10");
            AsignarHorario("Maria Santillana", "Domingo", "3/10");




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

