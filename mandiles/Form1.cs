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
    public partial class Form1 : Form
    {

        Dictionary<string, Label> cajas = new Dictionary<string, Label>();
        Dictionary<string, List<Label>> asignacionLabels = new Dictionary<string, List<Label>>();


        public Form1()
        {
            InitializeComponent();
            InicializarDiccionario();
            InicializarComboBox();
            InicializarAsignacionLabels();

        }

      
        private void InicializarDiccionario()
        {
            cajas["Caja 1"] = caja1;
            cajas["Caja 2"] = caja2;
            cajas["Caja 3"] = caja3;
            cajas["Caja 4"] = caja4;
            cajas["Caja 5"] = caja5;
            cajas["Caja 6"] = caja6;
            cajas["Caja 7"] = caja7;
            cajas["Caja 8"] = caja8;
            cajas["Caja 9"] = caja9;
            cajas["Caja 10"] = caja10;
            cajas["Caja 11"] = caja11;
            cajas["Caja 12"] = caja12;
            cajas["Caja 13"] = caja13;
            cajas["Caja 14"] = caja14;
            cajas["Caja 15"] = caja15;

        }
        private void InicializarAsignacionLabels()
        {
            asignacionLabels["Caja 1"] = new List<Label> { label17, label18, label19 };
            asignacionLabels["Caja 2"] = new List<Label> { label20, label21, label22 };
            asignacionLabels["Caja 3"] = new List<Label> { label23, label24, label25 };
            asignacionLabels["Caja 4"] = new List<Label> { label26, label27, label28 };
            asignacionLabels["Caja 5"] = new List<Label> { label29, label30, label31 };
            asignacionLabels["Caja 6"] = new List<Label> { label32, label33, label34 };
            asignacionLabels["Caja 7"] = new List<Label> { label35, label36, label37 };
            asignacionLabels["Caja 8"] = new List<Label> { label38, label39, label40 };
            asignacionLabels["Caja 9"] = new List<Label> { label41, label42, label43 };
            asignacionLabels["Caja 10"] = new List<Label> { label44, label45, label46 };
            asignacionLabels["Caja 11"] = new List<Label> { label47, label48, label49 };
            asignacionLabels["Caja 12"] = new List<Label> { label50, label51, label52 };
            asignacionLabels["Caja 13"] = new List<Label> { label53, label54, label55 };
            asignacionLabels["Caja 14"] = new List<Label> { label56, label57, label58 };
            asignacionLabels["Caja 15"] = new List<Label> { label59, label60, label61 };
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

        private void MoverCaja(ComboBox origen, ComboBox destino, string caja)
        {
            if (!destino.Items.Contains(caja))
            {
                destino.Items.Add(caja);
            }

            // Después, removemos el elemento de la lista original
            if (origen.Items.Contains(caja))
            {
                origen.Items.Remove(caja);
            }

            // Actualizar visualmente los ComboBox
            origen.Items.Remove(caja);
            origen.Refresh();
            destino.Refresh();

        }

        public void ActualizarAsignaciones(Dictionary<Label, List<string>> asignaciones)
        {
            foreach (var kvp in asignaciones)
            {
                Label cajaLabel = kvp.Key;

                // Buscar la clave (por ejemplo, "Caja 1") en el diccionario 'cajas' comparando por referencia.
                string cajaKey = cajas.FirstOrDefault(x => x.Value == cajaLabel).Key;
                if (string.IsNullOrEmpty(cajaKey))
                {
                    MessageBox.Show("No se encontró la caja correspondiente.");
                    continue;
                }

                List<string> empacadoresAsignados = kvp.Value;

                if (asignacionLabels.ContainsKey(cajaKey))
                {
                    List<Label> labelsCaja = asignacionLabels[cajaKey];
                    // Actualizar cada uno de los labels asignados a la caja
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
                else
                {
                    MessageBox.Show($"No se encontró asignación de labels para {cajaKey}");
                }
            }
        }

        public void AsignarEmpacador(string cajaName, string nombreEmpacador)
        {
            if (asignacionLabels.ContainsKey(cajaName))
            {
                List<Label> labelsAsignacion = asignacionLabels[cajaName];
                // Se asigna en el primer label vacío.
                foreach (Label lbl in labelsAsignacion)
                {
                    if (string.IsNullOrEmpty(lbl.Text))
                    {
                        lbl.Text = nombreEmpacador;
                        lbl.Visible = true;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show($"No se encontró la caja {cajaName} en asignacionLabels");
            }
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedItem == null) return;

            string selectedCaja = comboBox1.SelectedItem.ToString();

            // Cambia el color de la caja seleccionada
            CambiarColorCaja(selectedCaja, Color.Green);

            // Mueve la caja a comboBox2
            MoverCaja(comboBox1, comboBox2, selectedCaja);

            if (!comboBox3.Items.Contains(selectedCaja)) // Evitar duplicados
            {
                comboBox3.Items.Add(selectedCaja);
            }


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;

            string selectedCaja = comboBox2.SelectedItem.ToString();

            // Cambia el color de la caja a transparente (cerrada)
            CambiarColorCaja(selectedCaja, Color.Transparent);

            // Mueve la caja de regreso a comboBox1
            MoverCaja(comboBox2, comboBox1, selectedCaja);
        }

        private void CambiarColorCaja(string caja, Color color)
        {
            if (cajas.ContainsKey(caja))
            {
                cajas[caja].BackColor = color;
            }
        }

        


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox3.SelectedItem == null) return;

            string selectedCaja = comboBox3.SelectedItem.ToString();

            // Cambia el color de la caja a naranja (cerrada temporalmente)
            CambiarColorCaja(selectedCaja, Color.Orange);

            // Mueve la caja a comboBox1
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
    }
    
}
