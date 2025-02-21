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

namespace mandiles
{
    public partial class Form2 : Form
    {

        private Empacadores EmpacadoresHorario;

        private Form1 form1;
       
        List<string> empacadores = new List<string>();



        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            form1 = mainForm;
            EmpacadoresHorario= new Empacadores();
            CargarEmpacadoresDelDia();
        }

        private void CargarEmpacadoresDelDia()
        {
            // Usar la instancia EmpacadoresHorario que ya está definida
            List<string> empacadoresAsignados = EmpacadoresHorario.ObtenerEmpacadoresDelHorario(DateTime.Today);

            lbAsignadosHoy.Items.Clear();
            lbAsignadosHoy.Items.AddRange(empacadoresAsignados.ToArray());

            clbAusencias.Items.Clear();
            clbAusencias.Items.AddRange(empacadoresAsignados.ToArray());

        }

        private List<string> ObtenerEmpacadoresFinales()
        {
            List<string> empacadoresFinales = new List<string>();

            // Agregar todos los empacadores que estaban programados
            foreach (var item in lbAsignadosHoy.Items)
            {
                empacadoresFinales.Add(item.ToString());
            }

            // Remover los que están marcados como ausentes
            foreach (var item in clbAusencias.CheckedItems)
            {
                empacadoresFinales.Remove(item.ToString());
            }

            return empacadoresFinales;
        }



        public void CargarHorarioaDataGridView()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Definir las columnas del DataGridView
            dataGridView1.Columns.Add("Empacador", "Empacador");
            dataGridView1.Columns.Add("Lunes", "Lunes");
            dataGridView1.Columns.Add("Martes", "Martes");
            dataGridView1.Columns.Add("Miercoles", "Miercoles");
            dataGridView1.Columns.Add("Jueves", "Jueves");
            dataGridView1.Columns.Add("Viernes", "Viernes");
            dataGridView1.Columns.Add("Sabado", "Sabado");
            dataGridView1.Columns.Add("Domingo", "Domingo");

            // Obtener el horario desde la clase Empacadores
            var horarios = EmpacadoresHorario.ObtenerHorario();

            // Llenar el DataGridView con los datos
            foreach (var empacador in horarios)
            {
                string nombre = empacador.Key;
                string lunes = empacador.Value.ContainsKey("Lunes") ? empacador.Value["Lunes"] : "";
                string martes = empacador.Value.ContainsKey("Martes") ? empacador.Value["Martes"] : "";
                string miercoles = empacador.Value.ContainsKey("Miercoles") ? empacador.Value["Miercoles"] : "";
                string jueves = empacador.Value.ContainsKey("Jueves") ? empacador.Value["Jueves"] : "";
                string viernes = empacador.Value.ContainsKey("Viernes") ? empacador.Value["Viernes"] : "";
                string sabado = empacador.Value.ContainsKey("Sabado") ? empacador.Value["Sabado"] : "";
                string domingo = empacador.Value.ContainsKey("Domingo") ? empacador.Value["Domingo"] : "";

                dataGridView1.Rows.Add(nombre, lunes, martes, miercoles, jueves, viernes,sabado, domingo );
            }
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

            // 3) Obtener el día actual en español y normalizarlo
            string diaActual = DateTime.Now.ToString("dddd", new CultureInfo("es-ES"));
            // Normalizamos quitando tildes:
            diaActual = diaActual.Replace("á", "a")
                                 .Replace("é", "e")
                                 .Replace("í", "i")
                                 .Replace("ó", "o")
                                 .Replace("ú", "u");
            // Aseguramos que la primera letra esté en mayúscula:
            diaActual = char.ToUpper(diaActual[0]) + diaActual.Substring(1);
            // Ahora, si hoy es miércoles, diaActual quedará como "Miercoles" o "Miércoles"
            // Según el formato usado en las asignaciones. Aquí usaremos "Miércoles" (con tilde)
            // Por lo tanto, reemplazamos "Miercoles" por "Miércoles" si es el caso:
            if (diaActual.Equals("Miercoles", StringComparison.OrdinalIgnoreCase))
                diaActual = "Miercoles";
            if(diaActual.Equals("Jueves", StringComparison.OrdinalIgnoreCase))
                diaActual = "Jueves";

            // 4) Obtener los empacadores programados para hoy (excluyendo los que tienen "DESCANSO")
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
            
             CargarHorarioaDataGridView(); // Llenar el DataGridView
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

       
    }
}





