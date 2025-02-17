using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        }

        private Dictionary<string, List<string>> horario = new Dictionary<string, List<string>>();


        public void CargarHorarioaDataGridView()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Definir las columnas del DataGridView
            dataGridView1.Columns.Add("Empacador", "Empacador");
            dataGridView1.Columns.Add("Lunes", "Lunes");
            dataGridView1.Columns.Add("Martes", "Martes");
            dataGridView1.Columns.Add("Miércoles", "Miércoles");
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
            ListaEmpacadores.Items.Clear(); // Limpiar el ListBox antes de agregar los elementos
            foreach (var empacador in empacadores)
            {
                ListaEmpacadores.Items.Add(empacador); // Agregar cada empacador al ListBox
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

            // 3) Mezclar la lista de empacadores
            Random rng = new Random();
            List<string> empacadoresMezclados = empacadores.OrderBy(x => rng.Next()).ToList();

            // 4) Mezclar también la lista de cajas (para que la distribución sea aleatoria)
            List<Label> cajasBarajadas = cajasAbiertas.OrderBy(x => rng.Next()).ToList();

            // 5) Calcular la asignación equilibrada
            int totalEmpacadores = empacadoresMezclados.Count;
            int totalCajas = cajasBarajadas.Count;

            // Cantidad base por caja
            int baseCount = totalEmpacadores / totalCajas;
            // Cuántos "sobran" para dar 1 extra a algunas cajas
            int remainder = totalEmpacadores % totalCajas;

            // Diccionario: clave = Label de la caja, valor = lista de empacadores asignados
            Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
            foreach (var caja in cajasBarajadas)
            {
                asignaciones[caja] = new List<string>();
            }

            // 6) Asignar la base a cada caja
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

            // 7) Repartir los "sobrantes" en las primeras 'remainder' cajas
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

            // 9) Mostrar resumen en un MessageBox (opcional)
            string mensaje = "";
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
    }
}





