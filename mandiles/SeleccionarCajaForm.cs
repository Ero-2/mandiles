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
    public partial class SeleccionarCajaForm : Form
    {
        public string CajaSeleccionada { get; private set; } // Propiedad para almacenar la caja seleccionada
        public SeleccionarCajaForm(List<string> cajasCerradas)
        {
            InitializeComponent();

            // Crear botones dinámicos para cada caja cerrada
            foreach (var caja in cajasCerradas)
            {
                Button btnCaja = new Button
                {
                    Text = caja,
                    Tag = caja, // Almacenar el nombre de la caja en el Tag del botón
                    AutoSize = true,
                    Margin = new Padding(5)
                };

                btnCaja.Click += (sender, e) =>
                {
                    CajaSeleccionada = (string)((Button)sender).Tag; // Asignar la caja seleccionada
                    this.DialogResult = DialogResult.OK; // Cerrar el formulario con OK
                    this.Close();
                };

                flowLayoutPanel1.Controls.Add(btnCaja); // Agregar el botón al panel
            }

            // Botón Cancelar
            Button btnCancelar = new Button
            {
                Text = "Cancelar",
                AutoSize = true,
                Margin = new Padding(5)
            };

            btnCancelar.Click += (sender, e) =>
            {
                this.DialogResult = DialogResult.Cancel; // Cerrar el formulario con Cancel
                this.Close();
            };

            flowLayoutPanel1.Controls.Add(btnCancelar); // Agregar el botón Cancelar al panel
        }
    }

        
    
}
