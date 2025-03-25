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
using Microsoft.VisualBasic;
using MaterialSkin;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace mandiles
{
    public partial class Form1 : Form
    {



        private Dictionary<string, Label> cajas = new Dictionary<string, Label>();
        private Dictionary<string, List<Label>> asignacionLabels = new Dictionary<string, List<Label>>();
        private Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
        private Dictionary<string, Caja> cajass = new Dictionary<string, Caja>();
        private Dictionary<string, List<string>> temporalClosureEmpacadores = new Dictionary<string, List<string>>();

        private readonly string backupFile = "asignaciones_temp.dat";
        private System.Windows.Forms.Timer timerBackup; // Eli


        public Form1()
        {
            InitializeComponent();
            InicializarDiccionarios();
            InicializarComboBox(); 
             // <-- Añade esta línea
           
        }

       

      

        private void ActualizarComboBoxes()
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();
            comboBox5.Items.Clear();

            foreach (var caja in cajass.Values)
            {
                if (!caja.IsOpen)
                    comboBox1.Items.Add(caja.Nombre);
                else if (caja.IsOnBreak)
                    comboBox5.Items.Add(caja.Nombre);
                else
                    comboBox2.Items.Add(caja.Nombre);

                // Añadir a otros ComboBoxes
                if (!comboBox3.Items.Contains(caja.Nombre))
                    comboBox3.Items.Add(caja.Nombre);
                if (!comboBox4.Items.Contains(caja.Nombre))
                    comboBox4.Items.Add(caja.Nombre);
            }
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
                comboBox5.Items.Add($"Caja {i}");
            }

            // Manejar eventos
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;
            comboBox4.SelectedIndexChanged += comboBox4_SelectedIndexChanged;
            comboBox5.SelectedIndexChanged += comboBox5_SelectedIndexChanged;

        }

        private void CambiarColorCaja(string cajaName, Color color)
        {
            if (cajas.ContainsKey(cajaName))
                cajas[cajaName].BackColor = color;
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



            RegistrarCambio($"La {nombreCaja} fue cerrada. Empacadores reasignados: {string.Join(", ", empacadoresAReasignar)}");

            // REASIGNAMOS
            ReasignarEmpacadores(empacadoresAReasignar);
        }


        private void ReasignarEmpacadores(List<string> empacadores)
        {
            var openCajas = cajass.Values.Where(c => c.IsOpen && !c.IsOnBreak).OrderBy(c => c.Empacadores.Count).ToList();

            if (!openCajas.Any())
            {
                RegistrarCambio("No hay cajas abiertas para reasignar empacadores.");
                return;
            }

            foreach (var emp in empacadores)
            {
                foreach (var caja in openCajas)
                {
                    if (caja.Empacadores.Count < 3)
                    {
                        caja.Empacadores.Add(emp);
                        caja.UpdateUI();

                        RegistrarCambio($"{emp} fue reasignado a {caja.Nombre}");
                        break;
                    }
                }
            }
        }


        private void RegistrarCambio(string mensaje)
        {
            if (txtRegistroCambios.InvokeRequired)
            {
                txtRegistroCambios.Invoke(new Action(() => RegistrarCambio(mensaje)));
            }
            else
            {
                txtRegistroCambios.AppendText($"{DateTime.Now:HH:mm:ss} - {mensaje}\r\n");
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

                    RefrescarEmpacadoresAsignados();
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


        private void RedistribuirEmpacadoresNuevaCaja(string cajaNueva)
        {
            if (!cajass.ContainsKey(cajaNueva) || !cajass[cajaNueva].IsOpen)
                return;

            var cajasConMasDeUnEmpacador = cajass.Values
                .Where(c => c.IsOpen && c.Empacadores.Count > 1)
                .OrderByDescending(c => c.Empacadores.Count)
                .ToList();

            while (cajass[cajaNueva].Empacadores.Count < 2 && cajasConMasDeUnEmpacador.Count > 0)
            {
                foreach (var caja in cajasConMasDeUnEmpacador.ToList())
                {
                    if (caja.Empacadores.Count > 1)
                    {
                        var empacadorMovido = caja.Empacadores.Last();
                        caja.Empacadores.Remove(empacadorMovido);
                        cajass[cajaNueva].Empacadores.Add(empacadorMovido);

                        caja.UpdateUI();
                        cajass[cajaNueva].UpdateUI();
                    }

                    if (caja.Empacadores.Count == 1)
                        cajasConMasDeUnEmpacador.Remove(caja);

                    if (cajass[cajaNueva].Empacadores.Count >= 2)
                        break;
                }
            }
        }
        private void RefrescarEmpacadoresAsignados()
        {
            clbEmpacadoresForm1.Items.Clear();
            foreach (var caja in cajass.Values)
            {
                foreach (var emp in caja.Empacadores)
                {
                    if (!clbEmpacadoresForm1.Items.Contains(emp))
                        clbEmpacadoresForm1.Items.Add(emp, true); // Marcado como asignado
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;
            string cajaReabierta = comboBox1.SelectedItem.ToString();

            if (!cajass.ContainsKey(cajaReabierta)) return;
            Caja caja = cajass[cajaReabierta];

            // Determinar si estaba en descanso antes de reabrir
            bool estabaEnDescanso = caja.IsOnBreak;

            // Reactivar la caja (independientemente del estado anterior)
            caja.IsOpen = true;
            caja.IsOnBreak = false; // Resetear estado de descanso
            caja.MainLabel.BackColor = Color.Green;

            // Lógica de reasignación SOLO para cierres normales (no descansos)
            if (!estabaEnDescanso)
            {
                // Recuperar empacadores de cierre temporal si existen
                if (temporalClosureEmpacadores.ContainsKey(cajaReabierta))
                {
                    var empacadoresOriginales = temporalClosureEmpacadores[cajaReabierta].ToList();

                    foreach (var emp in empacadoresOriginales)
                    {
                        // Remover empacador de otras cajas
                        foreach (var otraCaja in cajass.Values.Where(c => c.Empacadores.Contains(emp)))
                        {
                            otraCaja.Empacadores.Remove(emp);
                            otraCaja.UpdateUI();
                        }

                        // Reasignar a la caja original si hay espacio
                        if (caja.Empacadores.Count < 3 && !caja.Empacadores.Contains(emp))
                        {
                            caja.Empacadores.Add(emp);
                        }
                        else
                        {
                            MessageBox.Show($"No hay espacio para {emp} en {cajaReabierta}");
                        }
                    }

                    temporalClosureEmpacadores[cajaReabierta].Clear();
                    caja.UpdateUI();
                }

                // Redistribuir solo si no era un descanso
                RedistribuirEmpacadoresNuevaCaja(cajaReabierta);
            }

            // Movimiento entre comboboxes
            MoverCaja(comboBox1, comboBox2, cajaReabierta);

            // Actualizar comboboxes secundarios
            var combos = new[] { comboBox3, comboBox4 };
            foreach (var combo in combos)
            {
                if (!combo.Items.Contains(cajaReabierta))
                    combo.Items.Add(cajaReabierta);
            }

            // Registrar en log
            string mensajeLog = estabaEnDescanso ?
                $"{DateTime.Now:HH:mm:ss} - {cajaReabierta} reabierta (descanso finalizado)" :
                $"{DateTime.Now:HH:mm:ss} - {cajaReabierta} reabierta";

            RegistrarCambio(mensajeLog);

            // Registrar empacadores solo si no era descanso
            if (!estabaEnDescanso)
            {
                foreach (var emp in caja.Empacadores)
                {
                    RegistrarCambio($"{DateTime.Now:HH:mm:ss} - {emp} reasignado a {cajaReabierta}");
                }
            }

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

            if (!cajass.ContainsKey(selectedCaja))
            {
                MessageBox.Show("La caja seleccionada no existe.");
                return;
            }

            Caja cajaCierre = cajass[selectedCaja];



            // Preguntamos si el cierre será mayor a 45 minutos
            var resultt = MessageBox.Show("¿El cierre será mayor a 45 minutos?",
                                         "Cierre Temporal",
                                         MessageBoxButtons.YesNo);

            // 1) Marcamos la caja como cerrada temporalmente
            cajaCierre.IsOpen = false;
            CambiarColorCaja(selectedCaja, Color.Orange);

            // 2) Guardamos los empacadores que tenía en un diccionario
            //    (así sabemos quiénes eran los 'originales' de esta caja)
            if (!temporalClosureEmpacadores.ContainsKey(selectedCaja))
                temporalClosureEmpacadores[selectedCaja] = new List<string>();

            // Copiamos la lista de empacadores a nuestro diccionario
            temporalClosureEmpacadores[selectedCaja].Clear();
            temporalClosureEmpacadores[selectedCaja].AddRange(cajaCierre.Empacadores);

            // 3) Limpiamos la caja
            cajaCierre.Empacadores.Clear();
            cajaCierre.UpdateUI();

            // 4) Si el cierre es mayor a 45 min, reasignar a otras cajas abiertas.
            //    Si no, van a una lista de espera.
            if (resultt == DialogResult.Yes)
            {
                ReasignarEmpacadores(temporalClosureEmpacadores[selectedCaja]);
            }
            else
            {
                AgregarAEmpacadoresEspera(temporalClosureEmpacadores[selectedCaja]);
            }

            // Mover la caja al comboBox correspondiente si quieres (opcional)
            MoverCaja(comboBox3, comboBox1, selectedCaja);

            RegistrarCambio($"{DateTime.Now:HH:mm:ss} - La caja {selectedCaja} fue cerrada temporalmente.");
            foreach (var emp in temporalClosureEmpacadores[selectedCaja])
            {
                RegistrarCambio($"{DateTime.Now:HH:mm:ss} - El empacador {emp} fue reasignado debido al cierre temporal de la caja {selectedCaja}.");
            }
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



        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verificar si hay una caja seleccionada en el ComboBox4
            if (comboBox4.SelectedItem == null)
            {
                MessageBox.Show("Seleccione una caja en ComboBox4.");
                return;
            }

            string selectedCaja = comboBox4.SelectedItem.ToString();

            // 1. Validar existencia de la caja seleccionada en el diccionario
            if (!cajass.ContainsKey(selectedCaja))
            {
                MessageBox.Show("La caja seleccionada no existe.");
                return;
            }

            // 2. Verificar si la caja está abierta
            Caja cajaAbierta = cajass[selectedCaja];
            if (!cajaAbierta.IsOpen)
            {
                MessageBox.Show("La caja seleccionada no está abierta.");
                return;
            }

            // 3. Obtener cajas cerradas
            var cajasCerradas = cajass.Where(c => !c.Value.IsOpen).Select(c => c.Key).ToList();

            if (cajasCerradas.Count == 0)
            {
                MessageBox.Show("No hay cajas cerradas para flotar.");
                return;
            }

            // Mostrar el formulario para seleccionar una caja cerrada
            using (var form = new SeleccionarCajaForm(cajasCerradas))
            {
                var resultado = form.ShowDialog();

                if (resultado == DialogResult.Cancel)
                {
                    return;  // El usuario canceló el proceso
                }

                string cajaFlotada = form.CajaSeleccionada; // Obtener la caja seleccionada

                if (!cajass.ContainsKey(cajaFlotada) || cajass[cajaFlotada].IsOpen)
                {
                    MessageBox.Show("La caja seleccionada para flotar no está cerrada.");
                    return;
                }

                // 4. Transferir empacadores
                Caja cajaFlotadaObj = cajass[cajaFlotada];

                // Copiar los empacadores de la caja abierta a la caja flotada
                cajaFlotadaObj.Empacadores.AddRange(cajaAbierta.Empacadores);

                // Limpiar los empacadores de la caja original
                cajaAbierta.Empacadores.Clear();

                // 5. Cambiar color de la caja flotada
                cajaFlotadaObj.MainLabel.BackColor = Color.SkyBlue;

                // 6. Actualizar la UI
                comboBox4.Items.Remove(selectedCaja);
                comboBox4.Items.Add(cajaFlotada);

                // 7. Actualizar estados correctamente
                cajaAbierta.IsOpen = false;
                cajaAbierta.MainLabel.BackColor = Color.Gray; // Marcar la caja original como cerrada

                cajaFlotadaObj.IsOpen = true; // Marcar la caja flotada como abierta

                // 8. Confirmar flotación
                MessageBox.Show($"La caja {selectedCaja} ha sido flotada a la caja {cajaFlotada}.");

                // 9. Actualizar la UI de ambas cajas
                cajaAbierta.UpdateUI();
                cajaFlotadaObj.UpdateUI();

                // 10. Mover la caja original al ComboBox1
                MoverCaja(comboBox4, comboBox1, selectedCaja);

                RegistrarCambio($"{DateTime.Now:HH:mm:ss} - La caja {selectedCaja} fue flotada a la caja {cajaFlotada}.");
                foreach (var emp in cajaFlotadaObj.Empacadores)
                {
                    RegistrarCambio($"{DateTime.Now:HH:mm:ss} - El empacador {emp} fue trasladado de {selectedCaja} a {cajaFlotada}.");
                }
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {// Centrar los TableLayoutPanel dentro del Panel
            foreach (Control control in panel1.Controls)
            {
                if (control is TableLayoutPanel)
                {
                    TableLayoutPanel tableLayout = (TableLayoutPanel)control;
                    tableLayout.Anchor = AnchorStyles.None; // Quitar anclajes para centrar
                    tableLayout.Location = new Point(
                        (panel1.ClientSize.Width - tableLayout.Width) / 5,
                        (panel1.ClientSize.Height - tableLayout.Height) / 5);
                }
            }


        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            lbHora.Text = DateTime.Now.ToLongTimeString();
            lbfecha.Text = DateTime.Now.ToLongDateString();
        }

        private void clbEmpacadoresForm1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Sólo actuar si se desmarca (es decir, pasa de Checked a Unchecked)
            if (e.NewValue == CheckState.Unchecked)
            {
                // Obtener el empacador que se está desmarcando
                string empacador = clbEmpacadoresForm1.Items[e.Index].ToString();

                // Removerlo de todas las cajas donde aparezca
                foreach (var cajaObj in cajass.Values)
                {
                    if (cajaObj.Empacadores.Contains(empacador))
                    {
                        cajaObj.Empacadores.Remove(empacador);
                        cajaObj.UpdateUI(); // Para limpiar el label en pantalla
                        RegistrarCambio($"{empacador} se retira de {cajaObj.Nombre}");
                    }
                }

                MessageBox.Show($"El empacador {empacador} ha sido retirado de todas las cajas.");
            }
        }



        private void panel2_Resize(object sender, EventArgs e)
        {
            // Centrar los TableLayoutPanel dentro del Panel cuando se redimensiona
            foreach (Control control in panel1.Controls)
            {
                if (control is TableLayoutPanel)
                {
                    TableLayoutPanel tableLayout = (TableLayoutPanel)control;
                    tableLayout.Location = new Point(
                        (panel1.ClientSize.Width - tableLayout.Width) / 5,
                        (panel1.ClientSize.Height - tableLayout.Height) / 5);
                }
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.SelectedItem == null) return;
            string selectedCaja = comboBox5.SelectedItem.ToString();

            if (!cajass.ContainsKey(selectedCaja)) return;

            Caja caja = cajass[selectedCaja];

            if (caja.IsOpen && !caja.IsOnBreak)
            {
                // 1. Marcar como en descanso
                caja.IsOnBreak = true;
                caja.MainLabel.BackColor = Color.Pink;

                // 2. Mover al ComboBox1 (abrir cajas) en lugar de ComboBox5
                MoverCaja(comboBox2, comboBox1, selectedCaja); // Desde abiertas (ComboBox2) a abrir (ComboBox1)

                // 3. Quitar de otros ComboBox si es necesario
                foreach (var combo in new[] { comboBox3, comboBox4, comboBox5 })
                {
                    if (combo.Items.Contains(selectedCaja))
                        combo.Items.Remove(selectedCaja);
                }

                RegistrarCambio($"{selectedCaja} entró en descanso (pendiente de reapertura)");
            }
        }

        public void AgregarEmpacadorACajasDisponibles(string empacador)
        {
            var cajasAbiertas = cajass.Values.Where(c => c.IsOpen && !c.IsOnBreak).OrderBy(c => c.Empacadores.Count);
            foreach (var caja in cajasAbiertas)
            {
                if (caja.Empacadores.Count < 3 && !caja.Empacadores.Contains(empacador))
                {
                    caja.Empacadores.Add(empacador);
                    caja.UpdateUI();
                    RegistrarCambio($"{empacador} agregado a {caja.Nombre}");
                    break;
                }
            }
            RefrescarEmpacadoresAsignados(); // Actualizar la CheckedListBox en Form1
        }


    }


}
