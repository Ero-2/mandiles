using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandiles
{
    public partial class Form1 : Form
    {

        private Dictionary<string, Label> cajas = new Dictionary<string, Label>();
        private Dictionary<string, List<Label>> asignacionLabels = new Dictionary<string, List<Label>>();
        private Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
        private Dictionary<string, Caja> cajass = new Dictionary<string, Caja>();
        private Dictionary<int, List<string>> CajasTemporales = new Dictionary<int, List<string>>();

        public Form1()
        {
            InitializeComponent();
            InicializarDiccionarios();
            InicializarComboBox();  

        }


        private void InicializarDiccionarios()
        {
            for (int i = 1; i <= 15; i++)
            {
                string cajaName = $"Caja {i}";
                Label cajaLabel = Controls.Find($"caja{i}", true).FirstOrDefault() as Label;
                cajas[cajaName] = cajaLabel;
                asignacionLabels[cajaName] = new List<Label>
                {
                    Controls.Find($"label{(i - 1) * 3 + 17}", true).FirstOrDefault() as Label,
                    Controls.Find($"label{(i - 1) * 3 + 18}", true).FirstOrDefault() as Label,
                    Controls.Find($"label{(i - 1) * 3 + 19}", true).FirstOrDefault() as Label
                };
                cajass[cajaName] = new Caja(cajaName, cajaLabel, asignacionLabels[cajaName]);


            }
        }

        private void InicializarComboBox()
        {
            for (int i = 1; i <= 15; i++)
            {
                comboBox1.Items.Add($"Caja {i}");
            }

            // Manejar eventos
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
        }

        private void CambiarColorCaja(string cajaName, Color color)
        {
            if (cajas.ContainsKey(cajaName))
                cajas[cajaName].BackColor = color;
        }

        private void OpenCaja(string nombreCaja)
        {
            if (!cajass.ContainsKey(nombreCaja)) return;
            Caja caja = cajass[nombreCaja];
            caja.IsOpen = true;
            caja.MainLabel.BackColor = Color.Green;
            caja.UpdateUI();
        }

        private void CloseCaja(string nombreCaja)
        {
            if (!cajass.ContainsKey(nombreCaja)) return;
            Caja cajaCerrada = cajass[nombreCaja];

            // Tomamos los empacadores de la caja y los quitamos de ahí
            List<string> empacadoresAReasignar = new List<string>(cajaCerrada.Empacadores);
            cajaCerrada.Empacadores.Clear();

            // Marcamos la caja como cerrada
            cajaCerrada.IsOpen = false;
            cajaCerrada.MainLabel.BackColor = Color.Transparent;
            cajaCerrada.UpdateUI();

            // REASIGNAMOS (solo una vez)
            ReasignarEmpacadores(empacadoresAReasignar);
        }

        

        private void ReasignarEmpacadores(List<string> empacadores)
        {
            var openCajas = cajass.Values.Where(c => c.IsOpen).OrderBy(c => c.Empacadores.Count).ToList();

            foreach (var emp in empacadores)
            {
                foreach (var caja in openCajas)
                {
                    if (caja.Empacadores.Count < 3)
                    {
                        caja.Empacadores.Add(emp);

                        // Cambiar color del empacador reasignado
                        Label labelEmpacador = caja.AsignacionLabels.FirstOrDefault(l => string.IsNullOrEmpty(l.Text));
                        if (labelEmpacador != null)
                        {
                            labelEmpacador.Text = emp;
                            labelEmpacador.ForeColor = Color.Red; // Color para identificar que fue movido

                            // Opcional: Restaurar color después de unos segundos
                            Task.Delay(3000).ContinueWith(_ => this.Invoke((Action)(() => labelEmpacador.ForeColor = Color.Black)));
                        }
                        break;
                    }
                }
            }

            foreach (var caja in openCajas)
            {
                caja.UpdateUI();
            }
        }


        

        private void MoverCaja(ComboBox origen, ComboBox destino, string caja)
        {
            if (!destino.Items.Contains(caja))
            {
                destino.Items.Add(caja);
            }

            if (origen.Items.Contains(caja))
            {
                origen.Items.Remove(caja); // Solo una vez
            }

            origen.Refresh();
            destino.Refresh();

        }

        public void ActualizarAsignaciones(Dictionary<Label, List<string>> newAssignments)
        {

            foreach (var kvp in newAssignments)
            {
                Label cajaLabel = kvp.Key;

                // Buscar la clave (por ejemplo, "Caja 1") en el diccionario 'cajas' comparando por referencia.
                string cajaKey = cajas.FirstOrDefault(x => x.Value == cajaLabel).Key;
                if (string.IsNullOrEmpty(cajaKey))
                {
                    MessageBox.Show("No se encontró la caja correspondiente.");
                    continue;
                }

                // Lista de empacadores asignados a esta caja (según la repartición de Form2)
                List<string> empacadoresAsignados = kvp.Value;

                // -------------------------------
                // 1) ACTUALIZAR LABELS EN LA UI
                // -------------------------------
                if (asignacionLabels.ContainsKey(cajaKey))
                {
                    List<Label> labelsCaja = asignacionLabels[cajaKey];
                    for (int i = 0; i < labelsCaja.Count; i++)
                    {
                        if (i < empacadoresAsignados.Count)
                        {
                            labelsCaja[i].Text = empacadoresAsignados[i];
                            labelsCaja[i].Visible = true;
                        }
                        else
                        {
                            labelsCaja[i].Text = "";
                            labelsCaja[i].Visible = false;
                        }
                    }
                }

                // -----------------------------------------------------
                // 2) SINCRONIZAR LA LISTA 'Empacadores' EN LA CLASE Caja
                // -----------------------------------------------------
                if (cajass.ContainsKey(cajaKey))
                {
                    Caja cajaObj = cajass[cajaKey];
                    cajaObj.Empacadores.Clear();
                    cajaObj.Empacadores.AddRange(empacadoresAsignados);
                }
            }
        }

        private void AgregarAEmpacadoresEspera(List<string> empacadores)
        {
            // Lista de espera (puedes implementarla como un List o cualquier otra estructura)
            foreach (var empacador in empacadores)
            {
                // Agregar a la lista de espera (no se hace nada visualmente en este caso)
                Console.WriteLine($"Empacador {empacador} en lista de espera.");
            }
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string selectedCaja = comboBox1.SelectedItem.ToString();

            // Llamar a OpenCaja para que la lógica interna de la caja sepa que está abierta
            OpenCaja(selectedCaja);

            // Si quieres, también puedes cambiar el color, pero en realidad OpenCaja ya lo hace:
            // CambiarColorCaja(selectedCaja, Color.Green);

            MoverCaja(comboBox1, comboBox2, selectedCaja);

            if (!comboBox3.Items.Contains(selectedCaja))
                comboBox3.Items.Add(selectedCaja);

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            string selectedCaja = comboBox2.SelectedItem.ToString();
            CambiarColorCaja(selectedCaja, Color.Transparent);
            MoverCaja(comboBox2, comboBox1, selectedCaja);
            CloseCaja(selectedCaja);
        }

       
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem == null) return;
            string selectedCaja = comboBox3.SelectedItem.ToString();

            var result = MessageBox.Show("¿El cierre será mayor a 45 minutos?", "Cierre Temporal", MessageBoxButtons.YesNo);
            Caja cajaCierre = cajass[selectedCaja];

            if (result == DialogResult.Yes)
            {
                // Si la respuesta es sí, reasignamos los empacadores a otras cajas con espacio
                
                ReasignarEmpacadores(cajaCierre.Empacadores);
            }
            else
            {
                // Si la respuesta es no, agregamos a los empacadores a una lista de espera
                AgregarAEmpacadoresEspera(cajaCierre.Empacadores);
            }



            CambiarColorCaja(selectedCaja, Color.Orange);
            MoverCaja(comboBox3, comboBox1, selectedCaja);
        }

        public List<Label> ObtenerCajasAbiertas()
        {
            List<Label> cajasAbiertas = new List<Label>();
            foreach (var kvp in cajas)
            {
                if (kvp.Value.BackColor == Color.Green)
                {
                    cajasAbiertas.Add(kvp.Value);
                }
            }
            return cajasAbiertas;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Extraemos los empacadores sin caja de todas las cajas cerradas
            List<string> empacadoresSinCaja = new List<string>();
            foreach (var kvp in asignaciones)
            {
                if (kvp.Key.BackColor == Color.Transparent) // La caja está cerrada
                {
                    empacadoresSinCaja.AddRange(kvp.Value);
                }
            }

            // Llamamos al método pasando la lista de empacadores sin caja
            ReasignarEmpacadores(empacadoresSinCaja);
        }

        
    }

}
