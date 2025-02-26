using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace mandiles
{
    public partial class Form2 : Form
    {

        private Empacadores EmpacadoresHorario;

        private Form1 form1;

        List<string> empacadores = new List<string>();

        private SQLiteConnection conexion;


        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            form1 = mainForm;
            EmpacadoresHorario = new Empacadores();
            ConectarBaseDeDatos();
            CargarEmpacadoresDelDia();
            ConectarBaseDeDatos();
        }

        private void CargarEmpacadoresDelDia()
        {
            try
            {
                string diaActual = DateTime.Now.ToString("dddd", new CultureInfo("es-ES")).ToUpper();
                MessageBox.Show($"Día actual: {diaActual}");

                // Verifica que EmpacadoresHorario está inicializado
                if (EmpacadoresHorario == null)
                {
                    MessageBox.Show("Error: EmpacadoresHorario no está inicializado");
                    return;
                }

                List<string> empacadores = EmpacadoresHorario.ObtenerEmpacadoresDelHorario(DateTime.Now);
                MessageBox.Show($"Empacadores encontrados para {diaActual}: {empacadores.Count}");

                lbAsignadosHoy.Items.Clear();
                clbAusencias.Items.Clear();

                lbAsignadosHoy.Items.AddRange(empacadores.ToArray());
                clbAusencias.Items.AddRange(empacadores.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en CargarEmpacadoresDelDia: {ex.Message}\n{ex.StackTrace}");
            }
        }


        private void ConectarBaseDeDatos()
        {
            try
            {
                string connectionString = "Data Source=C:\\Users\\eroer\\Downloads\\horariosEmpacadores.db;Version=3;";
                conexion = new SQLiteConnection(connectionString);
                conexion.Open();
                MessageBox.Show("Conexión exitosa a la base de datos");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar la base de datos: " + ex.Message);
            }
        }

        private void CerrarConexion()
        {
            if (conexion != null)
            {
                conexion.Close();
                MessageBox.Show("Conexión cerrada");
            }
        }

        private void CargarHorarios()
        {
            try
            {
                string query = "SELECT * FROM HorarioEmpacadores"; // Ajusta el nombre de la tabla
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conexion);
                System.Data.DataTable dt = new System.Data.DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt; // dataGridView1 es el nombre de tu control
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private void CargarHorariosEnClaseEmpacadores()
        {
            // Ejemplo: asumiendo que tu tabla se llama HorarioEmpacadores y que
            // contiene columnas: Nombre, Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo.
            string query = "SELECT Empacador, Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo FROM HorarioEmpacadores";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conexion))
            {
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nombre = reader["Empacador"].ToString();

                        // OJO: Normaliza cada string de día para que coincida con
                        // la forma en que luego vas a buscar (MARTES, MIERCOLES, etc.).
                        // Aquí puedes simplemente asignar tal cual, si en tu clase
                        // Empacadores siempre usas mayúsculas para el diccionario.
                        EmpacadoresHorario.AsignarHorario(nombre, "LUNES", reader["Lunes"].ToString());
                        EmpacadoresHorario.AsignarHorario(nombre, "MARTES", reader["Martes"].ToString());
                        EmpacadoresHorario.AsignarHorario(nombre, "MIERCOLES", reader["Miercoles"].ToString());
                        EmpacadoresHorario.AsignarHorario(nombre, "JUEVES", reader["Jueves"].ToString());
                        EmpacadoresHorario.AsignarHorario(nombre, "VIERNES", reader["Viernes"].ToString());
                        EmpacadoresHorario.AsignarHorario(nombre, "SABADO", reader["Sabado"].ToString());
                        EmpacadoresHorario.AsignarHorario(nombre, "DOMINGO", reader["Domingo"].ToString());
                    }
                }
            }
        }






        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CerrarConexion();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string nombreEmpacador = textBox1.Text.Trim(); // Obtener el valor ingresado en el TextBox

            // Verificar que el nombre no esté vacío y no esté repetido
            if (!string.IsNullOrEmpty(nombreEmpacador) && !empacadores.Contains(nombreEmpacador))
            {
                empacadores.Add(nombreEmpacador); // Agregar el nombre a la lista
                ActualizarListaEmpacadores(); // Actualizar el ListBox

               
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un nombre válido o uno que no esté repetido.");
            }
        }

        private void ActualizarListaEmpacadores()
        {
            lbAsignadosHoy.Items.Clear(); // Limpiar el ListBox antes de agregar los elementos
            foreach (var empacador in empacadores)
            {
                lbAsignadosHoy.Items.Add(empacador); // Agregar cada empacador al ListBox
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // 1) Obtener la instancia de Form1
            Form1 form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 == null)
            {
                MessageBox.Show("No se encontró el Form1.");
                return;
            }

            // 2) Obtener las cajas abiertas (BackColor == Green)
            List<Label> cajasAbiertas = form1.ObtenerCajasAbiertas();
            if (cajasAbiertas.Count == 0)
            {
                MessageBox.Show("No hay cajas abiertas para asignar.");
                return;
            }

            string diaActual = DateTime.Now.ToString("dddd", new CultureInfo("es-ES"));
            // Normalizar sin tildes y en mayúsculas:
            diaActual = diaActual
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .ToUpper(); // Convertir a mayúsculas (ej: "MARTES")

            // 4) Obtener los empacadores programados para hoy
            var horariosDict = EmpacadoresHorario.ObtenerHorario();
            List<string> empacadoresHoy = new List<string>();

            foreach (var emp in horariosDict)
            {
                if (emp.Value.ContainsKey(diaActual) &&
                    !string.IsNullOrEmpty(emp.Value[diaActual]) &&
                    emp.Value[diaActual].ToUpper() != "DESCANSO")
                {
                    empacadoresHoy.Add(emp.Key);
                }
            }


            // Verificar si hay empacadores para asignar
            if (empacadoresHoy.Count == 0)
            {
                MessageBox.Show($"No hay empacadores programados para {diaActual}.");
                return;
            }

            // 5) Mezclar la lista de empacadores
            Random rng = new Random();
            List<string> empacadoresMezclados = empacadoresHoy.OrderBy(x => rng.Next()).ToList();

            // 6) Mezclar la lista de cajas abiertas
            List<Label> cajasBarajadas = cajasAbiertas.OrderBy(x => rng.Next()).ToList();

            // 7) Calcular la asignación equilibrada
            int totalEmpacadores = empacadoresMezclados.Count;
            int totalCajas = cajasBarajadas.Count;
            int baseCount = totalEmpacadores / totalCajas;
            int remainder = totalEmpacadores % totalCajas;

            Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
            foreach (var caja in cajasBarajadas)
            {
                asignaciones[caja] = new List<string>();
            }

            int currentIndex = 0;
            foreach (var caja in cajasBarajadas)
            {
                for (int i = 0; i < baseCount; i++)
                {
                    if (currentIndex < empacadoresMezclados.Count)
                    {
                        asignaciones[caja].Add(empacadoresMezclados[currentIndex]);
                        currentIndex++;
                    }
                }
            }

            for (int i = 0; i < remainder; i++)
            {
                if (currentIndex < empacadoresMezclados.Count)
                {
                    asignaciones[cajasBarajadas[i]].Add(empacadoresMezclados[currentIndex]);
                    currentIndex++;
                }
            }

            // 8) Actualizar en Form1 los labels de asignación
            form1.ActualizarAsignaciones(asignaciones);

            // 9) Mostrar resumen en un MessageBox
            string mensaje = $"Asignación de empacadores para {diaActual}:\n\n";
            foreach (var kvp in asignaciones)
            {
                string cajaName = kvp.Key.Text;
                string listaEmp = string.Join(", ", kvp.Value);
                mensaje += $"{cajaName}: {listaEmp}\n";
            }
            MessageBox.Show(mensaje, "Asignación de Empacadores");
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            ConectarBaseDeDatos();
            CargarHorarios();
            CargarHorariosEnClaseEmpacadores();
            CargarEmpacadoresDelDia();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

       
    }
}





