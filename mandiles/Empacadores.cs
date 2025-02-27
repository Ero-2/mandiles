using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandiles
{
    // Clase Empacadores
    internal class Empacadores
    {
        private Dictionary<string, Dictionary<string, string>> horarioEmpacadores = new Dictionary<string, Dictionary<string, string>>();
        private string connectionString = "Data Source=C:\\Users\\eroer\\Downloads\\horariosEmpacadores.db;Version=3;";

        public void CargarDatosDesdeSQLite()
        {
            horarioEmpacadores.Clear(); // Limpiar datos previos

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM HorarioEmpacadores";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nombre = reader["EMPACADOR"].ToString();

                            if (!horarioEmpacadores.ContainsKey(nombre))
                            {
                                horarioEmpacadores[nombre] = new Dictionary<string, string>();
                            }

                            // Recorrer los días de la semana
                            string[] dias = { "LUNES", "MARTES", "MIERCOLES", "JUEVES", "VIERNES", "SABADO", "DOMINGO" };
                            foreach (var dia in dias)
                            {
                                horarioEmpacadores[nombre][dia] = reader[dia].ToString();
                            }
                        }
                    }
                }
            }
        }
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
            string diaSemana = fecha.ToString("dddd", new CultureInfo("es-ES")).ToUpper().Trim();

            // Asegurarse de que los nombres de los días coincidan con los de la BD
            Dictionary<string, string> conversionDias = new Dictionary<string, string>
    {
        { "LUNES", "LUNES" },
        { "MARTES", "MARTES" },
        { "MIÉRCOLES", "MIERCOLES" },
        { "JUEVES", "JUEVES" },
        { "VIERNES", "VIERNES" },
        { "SÁBADO", "SABADO" },
        { "DOMINGO", "DOMINGO" }
    };

            if (conversionDias.ContainsKey(diaSemana))
            {
                diaSemana = conversionDias[diaSemana];
            }
            else
            {
                MessageBox.Show($"Error: Día no reconocido ({diaSemana}).");
                return new List<string>();
            }

            List<string> empacadoresDelDia = new List<string>();

            foreach (var emp in horarioEmpacadores)
            {
                if (emp.Value.ContainsKey(diaSemana))
                {
                    string horario = emp.Value[diaSemana].Trim().ToUpper();

                    if (!string.IsNullOrEmpty(horario) && horario != "DESCANSO")
                    {
                        empacadoresDelDia.Add(emp.Key);
                    }
                }
            }
            return empacadoresDelDia;
        }

    }
}

