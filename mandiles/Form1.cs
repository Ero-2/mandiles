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

namespace mandiles
{
    public partial class Form1 : Form
    {

        private Dictionary<string, Label> cajas = new Dictionary<string, Label>();
        private Dictionary<string, List<Label>> asignacionLabels = new Dictionary<string, List<Label>>();
        private Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
        private Dictionary<string, Caja> cajass = new Dictionary<string, Caja>();
        private Dictionary<string, List<string>> temporalClosureEmpacadores = new Dictionary<string, List<string>>();

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
            comboBox4.SelectedIndexChanged += comboBox4_SelectedIndexChanged;
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

            // REASIGNAMOS (solo una vez)
            ReasignarEmpacadores(empacadoresAReasignar);
        }


        private void ReasignarEmpacadores(List<string> empacadores)
        {
            // Solo cajas que están abiertas (IsOpen = true)
            var openCajas = cajass.Values
                                  .Where(c => c.IsOpen)
                                  .OrderBy(c => c.Empacadores.Count)
                                  .ToList();

            if (!openCajas.Any())
            {
                MessageBox.Show("No hay cajas abiertas para reasignar empacadores.");
                return;
            }

            // Recorremos una copia de la lista para evitar la excepción de “colección modificada”
            var copiaEmpacadores = new List<string>(empacadores);

            foreach (var emp in copiaEmpacadores)
            {
                bool reasignado = false;
                foreach (var caja in openCajas)
                {
                    // Buscamos una caja con espacio (3 o el límite que manejes)
                    if (caja.Empacadores.Count < 3)
                    {
                        caja.Empacadores.Add(emp);
                        reasignado = true;

                        // Actualizar UI
                        caja.UpdateUI();

                        // Opcional: Cambiar color de label, etc.
                        break;
                    }
                }
                // Si no se reasignó, puedes hacer algo extra (lista de espera, mensaje, etc.)
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




        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comboBox1.SelectedItem == null) return;
            string cajaReabierta = comboBox1.SelectedItem.ToString();

            // Verificamos si la caja existe en el diccionario
            if (cajass.ContainsKey(cajaReabierta))
            {
                cajass[cajaReabierta].IsOpen = true;
                cajass[cajaReabierta].MainLabel.BackColor = Color.Green;

                // Si la caja tiene empacadores guardados por el cierre temporal
                if (temporalClosureEmpacadores.ContainsKey(cajaReabierta))
                {
                    var empacadoresOriginales = temporalClosureEmpacadores[cajaReabierta];

                    foreach (var emp in empacadoresOriginales.ToList()) // Usamos ToList() para evitar modificar la colección mientras iteramos
                    {
                        // Removemos al empacador de cualquier otra caja donde haya sido reasignado
                        foreach (var caja in cajass.Values)
                        {
                            if (caja.Empacadores.Contains(emp))
                            {
                                caja.Empacadores.Remove(emp);
                                caja.UpdateUI();
                                break; // Solo está en una caja, así que podemos salir del loop
                            }
                        }

                        // Reasignamos el empacador a la caja reabierta
                        if (cajass[cajaReabierta].Empacadores.Count < 3 && !cajass[cajaReabierta].Empacadores.Contains(emp))
                        {
                            cajass[cajaReabierta].Empacadores.Add(emp);
                        }
                        else
                        {
                            MessageBox.Show($"No hay espacio para el empacador {emp} en la caja {cajaReabierta}.");
                        }
                    }

                    // Limpiamos la lista después de reasignarlos
                    temporalClosureEmpacadores[cajaReabierta].Clear();

                    // Actualizamos la UI de la caja reabierta
                    cajass[cajaReabierta].UpdateUI();
                }

                // **Nueva lógica: Redistribuir empacadores si es una nueva caja abierta**
                RedistribuirEmpacadoresNuevaCaja(cajaReabierta);

                // Mover la caja entre los ComboBox correspondientes
                MoverCaja(comboBox1, comboBox2, cajaReabierta);

                // Agregar la caja al ComboBox3 si no está presente
                if (!comboBox3.Items.Contains(cajaReabierta))
                    comboBox3.Items.Add(cajaReabierta);
                // Agregar la caja al ComboBox3 si no está presente
                if (!comboBox4.Items.Contains(cajaReabierta))
                    comboBox4.Items.Add(cajaReabierta);
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
            }


        }


    }


}
