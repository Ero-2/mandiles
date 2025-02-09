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

        private Form1 form1;
       
        List<string> empacadores = new List<string>();

        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            form1 = mainForm;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

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
            // Obtener la instancia de Form1
            Form1 form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 == null)
            {
                MessageBox.Show("No se encontró el Form1.");
                return;
            }

            // Obtener las cajas abiertas en Form1 (por ejemplo, aquellas con BackColor == Color.Green)
            List<Label> cajasAbiertas = form1.ObtenerCajasAbiertas();
            if (cajasAbiertas.Count == 0)
            {
                MessageBox.Show("No hay cajas abiertas para asignar.");
                return;
            }

            // Mezclar aleatoriamente la lista de empacadores
            Random rng = new Random();
            List<string> empacadoresMezclados = empacadores.OrderBy(x => rng.Next()).ToList();

            // Creamos un diccionario para guardar las asignaciones:
            // clave: Label de la caja; valor: lista de empacadores asignados.
            Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
            foreach (Label caja in cajasAbiertas)
            {
                asignaciones[caja] = new List<string>();
            }

            // Distribución round-robin (o de la forma que prefieras)
            int countBoxes = cajasAbiertas.Count;
            int index = 0;
            foreach (string emp in empacadoresMezclados)
            {
                bool asignado = false;
                for (int j = 0; j < countBoxes; j++)
                {
                    int candidateIndex = (index + j) % countBoxes;
                    Label candidateBox = cajasAbiertas[candidateIndex];
                    if (asignaciones[candidateBox].Count < 3)
                    {
                        asignaciones[candidateBox].Add(emp);
                        asignado = true;
                        index = (candidateIndex + 1) % countBoxes;
                        break;
                    }
                }
                if (!asignado)
                {
                    MessageBox.Show($"No se pudo asignar al empacador {emp}: todas las cajas abiertas tienen 3 empacadores.");
                }
            }

            // Actualizar en Form1 los labels de asignación
            form1.ActualizarAsignaciones(asignaciones);

            // Opcional: mostrar resumen en un MessageBox
            string mensaje = "";
            foreach (var kvp in asignaciones)
            {
                string cajaName = kvp.Key.Text;
                string listaEmp = string.Join(", ", kvp.Value);
                mensaje += $"Caja {cajaName}: {listaEmp}\n";
            }
            MessageBox.Show(mensaje, "Asignación de Empacadores");
        }
    }
}





