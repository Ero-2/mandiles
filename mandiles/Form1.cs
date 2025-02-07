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

        public Form1()
        {
            InitializeComponent();
            InicializarComboBox();

        }
        private void InicializarComboBox()
        {
            // Agregar opción de Caja 1 en el ComboBox
            comboBox1.Items.Add("Caja 1");
            comboBox1.Items.Add("Caja 2");
            comboBox1.Items.Add("Caja 3");
            comboBox1.Items.Add("Caja 4");
            comboBox1.Items.Add("Caja 5");
            comboBox1.Items.Add("Caja 6");
            comboBox1.Items.Add("Caja 7");
            comboBox1.Items.Add("Caja 8");
            comboBox1.Items.Add("Caja 9");
            comboBox1.Items.Add("Caja 10");
            comboBox1.Items.Add("Caja 11");
            comboBox1.Items.Add("Caja 12");
            comboBox1.Items.Add("Caja 13");
            comboBox1.Items.Add("Caja 14");
            comboBox1.Items.Add("Caja 15");

            // Manejar el evento de selección
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
        }




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

                string selectedCaja = comboBox1.SelectedItem.ToString();

            // Verificar cuál caja fue seleccionada y cambiar colores
            if (selectedCaja == "Caja 1")
            {
                caja1.BackColor = Color.Green; // Fondo de la caja grande
                
            }
            else if (selectedCaja == "Caja 2")
            {
                caja2.BackColor = Color.Green;
                
            }
            else if (selectedCaja == "Caja 3")
            {
                caja3.BackColor = Color.Green;

            }
            else if (selectedCaja == "Caja 4")
            {
                caja4.BackColor = Color.Green;

            }
            else if (selectedCaja == "Caja 5")
            {
                caja5.BackColor = Color.Green;

            }
            else if (selectedCaja == "Caja 6")
            {
                caja6.BackColor = Color.Green;

            }
            else if (selectedCaja == "Caja 7")
            {
                caja7.BackColor = Color.Green;

            }
            else if (selectedCaja == "Caja 8")
            {
                caja8.BackColor = Color.Green;


            }
        }
    }
}
