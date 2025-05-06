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


        private List<string> empacadoresEnEspera = new List<string>();
        private Dictionary<string, Label> cajas = new Dictionary<string, Label>();
        private Dictionary<string, List<Label>> asignacionLabels = new Dictionary<string, List<Label>>();
        private Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
        private Dictionary<string, Caja> cajass = new Dictionary<string, Caja>();
        private Dictionary<string, List<string>> temporalClosureEmpacadores = new Dictionary<string, List<string>>();
        public static Dictionary<Label, DateTime> highlightedLabels = new Dictionary<Label, DateTime>();
        private System.Windows.Forms.Timer highlightTimer;
        private Dictionary<string, List<string>> previousAssignments = new Dictionary<string, List<string>>();
        private Dictionary<string, string> empacadorCajaAnterior = new Dictionary<string, string>();
        private Stack<StateSnapshot> undoHistory = new Stack<StateSnapshot>();




        public Form1()
        {
            InitializeComponent();
            InicializarDiccionarios();
            InicializarComboBox();

            highlightTimer = new System.Windows.Forms.Timer();
            highlightTimer.Interval = 1000; // Verificar cada segundo
            highlightTimer.Tick += HighlightTimer_Tick;
            highlightTimer.Start(); // ¡Asegúrate de que esto no esté comentado!

        }

        public class StateSnapshot
        {
            public Dictionary<string, CajaState> Cajas { get; set; } = new Dictionary<string, CajaState>();
            public List<string> EmpacadoresEnEspera { get; set; } = new List<string>();
        }

        public class CajaState
        {
            public bool IsOpen { get; set; }
            public bool IsOnBreak { get; set; }
            public List<string> Empacadores { get; set; } = new List<string>();
        }

        private void SaveCurrentStateToUndoStack()
        {
            var snapshot = new StateSnapshot
            {
                EmpacadoresEnEspera = new List<string>(empacadoresEnEspera)
            };

            foreach (var cajaKvp in cajass)
            {
                var caja = cajaKvp.Value;
                snapshot.Cajas[caja.Nombre] = new CajaState
                {
                    IsOpen = caja.IsOpen,
                    IsOnBreak = caja.IsOnBreak,
                    Empacadores = new List<string>(caja.Empacadores)
                };
            }

            undoHistory.Push(snapshot);
        }

        private void UndoLastOperation()
        {
            if (undoHistory.Count == 0)
            {
                MessageBox.Show("No hay operaciones anteriores para deshacer.");
                return;
            }

            var lastState = undoHistory.Pop();

            // Restaurar empacadores en espera
            empacadoresEnEspera.Clear();
            empacadoresEnEspera.AddRange(lastState.EmpacadoresEnEspera);

            // Restaurar estado de las cajas
            foreach (var cajaKvp in cajass)
            {
                string nombreCaja = cajaKvp.Key;
                Caja caja = cajaKvp.Value;

                if (lastState.Cajas.TryGetValue(nombreCaja, out var state))
                {
                    caja.IsOpen = state.IsOpen;
                    caja.IsOnBreak = state.IsOnBreak;
                    caja.Empacadores.Clear();
                    caja.Empacadores.AddRange(state.Empacadores);
                    caja.UpdateUI();

                    // Actualizar color visual de la caja según su estado
                    if (caja.IsOpen && !caja.IsOnBreak)
                        caja.MainLabel.BackColor = Color.Green;
                    else if (caja.IsOpen && caja.IsOnBreak)
                        caja.MainLabel.BackColor = Color.Pink;
                    else if (!caja.IsOpen)
                        caja.MainLabel.BackColor = Color.Transparent;
                }
            }

            // Refrescar interfaz
            RefrescarListaEspera();
            RefrescarEmpacadoresAsignados();
            InicializarComboBox(); // Re-inicializar comboboxes si es necesario

            MessageBox.Show("Operación revertida. Se ha restaurado el estado anterior.");
        }

        private void HighlightTimer_Tick(object sender, EventArgs e)
        {
            var labelsToRemove = new List<Label>();
            foreach (var kvp in highlightedLabels)
            {
                if ((DateTime.Now - kvp.Value).TotalSeconds >= 60)
                {
                    kvp.Key.ForeColor = SystemColors.ControlText;
                    kvp.Key.BorderStyle = BorderStyle.None;
                    labelsToRemove.Add(kvp.Key);
                }
            }
            foreach (var label in labelsToRemove)
            {
                highlightedLabels.Remove(label);
            }
        }

        private void ResaltarReasignacionesExclusivas()
        {
            // 1. Limpiar resaltados anteriores
            foreach (var label in highlightedLabels.Keys.ToList())
            {
                label.ForeColor = SystemColors.ControlText;
                label.BorderStyle = BorderStyle.None;
                highlightedLabels.Remove(label);
            }

            // 2. Identificar cambios entre asignaciones anteriores y actuales
            var cambios = new Dictionary<string, string>(); // { Empacador, NuevaCaja }
            foreach (var cajaActual in cajass)
            {
                foreach (var empacador in cajaActual.Value.Empacadores)
                {
                    if (previousAssignments.TryGetValue(cajaActual.Key, out var empacadoresAnteriores))
                    {
                        if (!empacadoresAnteriores.Contains(empacador))
                        {
                            cambios[empacador] = cajaActual.Key; // Empacador reasignado
                        }
                    }
                }
            }

            // 3. Pintar los labels de los empacadores reasignados
            foreach (var cambio in cambios)
            {
                string empacador = cambio.Key;
                string cajaDestino = cambio.Value;

                if (asignacionLabels.TryGetValue(cajaDestino, out var labelsCaja))
                {
                    foreach (var label in labelsCaja)
                    {
                        if (label.Text == empacador)
                        {
                            label.ForeColor = Color.Purple;  // Color para reasignados
                            label.BorderStyle = BorderStyle.Fixed3D;
                            highlightedLabels[label] = DateTime.Now; // Registrar para el temporizador
                            break;
                        }
                    }
                }
            }

            // 4. Actualizar asignaciones anteriores para futuras comparaciones
            foreach (var cajaKvp in cajass)
            {
                previousAssignments[cajaKvp.Key] = new List<string>(cajaKvp.Value.Empacadores);
            }
        }


        private void ResaltarLabel(Label label)
        {
            if (label == null) return;

            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => ResaltarLabel(label)));
                return;
            }

            // Forzar la actualización visual del label
            label.ForeColor = Color.Purple;
            label.BorderStyle = BorderStyle.Fixed3D;
            label.Refresh(); // Forzar repintado inmediato

            // Registrar en el diccionario de labels resaltados
            Form1.highlightedLabels[label] = DateTime.Now;

            // Registrar en el log para depuración
            Console.WriteLine($"Label resaltado: {label.Text} a las {DateTime.Now}");
            RegistrarCambio($"Empacador {label.Text} fue reasignado y resaltado");
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

            foreach (var cajaKvp in cajass)
            {
                previousAssignments[cajaKvp.Key] = new List<string>(cajaKvp.Value.Empacadores);
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

        private void ClearHighlightsForEmpacador(string empacador)
        {
            var labelsToRemove = Form1.highlightedLabels.Keys
                .Where(label => label.Text == empacador)
                .ToList();

            foreach (var label in labelsToRemove)
            {
                label.ForeColor = SystemColors.ControlText;
                label.BorderStyle = BorderStyle.None;
                Form1.highlightedLabels.Remove(label);
            }
        }





        private void CloseCaja(string nombreCaja)
        {
            SaveCurrentStateToUndoStack();

            if (!cajass.ContainsKey(nombreCaja)) return;
            Caja cajaCerrada = cajass[nombreCaja];

            // Save current state before closing the box
            var oldPreviousAssignments = previousAssignments.ToDictionary(
                kvp => kvp.Key,
                kvp => new List<string>(kvp.Value)
            );

            // Tomamos los empacadores de la caja y los quitamos de ahí
            List<string> empacadoresAReasignar = new List<string>(cajaCerrada.Empacadores);
            cajaCerrada.Empacadores.Clear();

            // Marcamos la caja como cerrada
            cajaCerrada.IsOpen = false;
            cajaCerrada.MainLabel.BackColor = Color.Transparent;
            cajaCerrada.UpdateUI();

            RegistrarCambio($"La {nombreCaja} fue cerrada. Empacadores reasignados: {string.Join(", ", empacadoresAReasignar)}");

            // Restore the previous assignments before reassignment
            previousAssignments = oldPreviousAssignments;

            // REASIGNAMOS
            ReasignarEmpacadores(empacadoresAReasignar);
            BalancearEmpacadores();
            ResaltarReasignacionesExclusivas();
            RefrescarListaEspera();
            // La llamada a CheckAndHighlightReassignments se hace dentro de ReasignarEmpacadores
        }



        private void ReasignarEmpacadores(List<string> empacadores)
        {
            if (empacadores == null || !empacadores.Any()) return;

            // Guardar asignaciones previas
            var empacadorCajaAnterior = new Dictionary<string, string>();
            foreach (var kvp in cajass)
            {
                foreach (var emp in kvp.Value.Empacadores)
                {
                    empacadorCajaAnterior[emp] = kvp.Key;
                }
            }

            var openCajas = cajass.Values
                .Where(c => c.IsOpen && !c.IsOnBreak)
                .OrderBy(c => c.Empacadores.Count)
                .ToList();

            if (!openCajas.Any())
            {
                foreach (var emp in empacadores)
                {
                    if (!empacadoresEnEspera.Contains(emp))
                    {
                        empacadoresEnEspera.Add(emp);
                        RegistrarCambio($"{emp} agregado a la lista de espera (no hay cajas disponibles)");
                    }
                }

                MessageBox.Show(
                    $"No hay cajas abiertas disponibles.\r\n" +
                    $"Hay {empacadoresEnEspera.Count} empacadores en espera.\r\n" +
                    "Favor de abrir una nueva caja para asignarlos.",
                    "¡Atención!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                RefrescarListaEspera();
                return;
            }

            // Limpiar resaltados anteriores
            foreach (var emp in empacadores.ToList())
            {
                ClearHighlightsForEmpacador(emp);

                bool asignado = false;
                var cajasOrdenadas = openCajas
                    .Where(c => c.Empacadores.Count < 3)
                    .OrderBy(c => c.Empacadores.Count)
                    .ToList();

                foreach (var caja in cajasOrdenadas)
                {
                    if (caja.Empacadores.Count < 3 && !caja.Empacadores.Contains(emp))
                    {
                        caja.Empacadores.Add(emp);
                        caja.UpdateUI(); // Primero actualiza visual

                        RegistrarCambio($"{emp} fue reasignado a {caja.Nombre}");
                        empacadoresEnEspera.Remove(emp);
                        asignado = true;

                        // Si el empacador fue reasignado a una caja distinta, resaltar
                        if (empacadorCajaAnterior.TryGetValue(emp, out string cajaAnterior))
                        {
                            if (cajaAnterior != caja.Nombre)
                            {
                                // Buscar la posición correcta del empacador en la lista de labels
                                int empIndex = caja.Empacadores.IndexOf(emp);
                                if (empIndex >= 0 && empIndex < caja.AsignacionLabels.Count)
                                {
                                    Label labelToHighlight = caja.AsignacionLabels[empIndex];
                                    Console.WriteLine($"Resaltando durante reasignación: {emp} en {caja.Nombre}");
                                    ResaltarLabel(labelToHighlight);
                                }
                            }
                        }
                        else
                        {
                            // Si es nuevo, también resaltar
                            foreach (var label in caja.AsignacionLabels)
                            {
                                if (string.IsNullOrEmpty(label.Text))
                                {
                                    label.Text = emp;
                                    ResaltarLabel(label); // Resalta el label asignado
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }

                if (!asignado && !empacadoresEnEspera.Contains(emp))
                {
                    empacadoresEnEspera.Add(emp);
                    RegistrarCambio($"{emp} agregado a la lista de espera (cajas llenas)");
                }
            }

            RefrescarListaEspera();
            ResaltarReasignacionesExclusivas();
        }


        private void CheckAndHighlightReassignments()
        {
            // Diccionario actual después de reasignar
            Dictionary<string, string> empacadorCajaActual = new Dictionary<string, string>();

            foreach (var caja in cajass.Values)
            {
                foreach (var emp in caja.Empacadores)
                {
                    empacadorCajaActual[emp] = caja.Nombre;
                }
            }

            foreach (var empacador in empacadorCajaActual.Keys)
            {
                // Verifica si estaba antes y cambió de caja
                if (empacadorCajaAnterior.ContainsKey(empacador) &&
                    empacadorCajaAnterior[empacador] != empacadorCajaActual[empacador])
                {
                    // Obtener la caja actual donde está el empacador
                    string cajaNueva = empacadorCajaActual[empacador];

                    // Buscar el label correspondiente para resaltarlo
                    if (cajass.ContainsKey(cajaNueva))
                    {
                        Caja cajaObj = cajass[cajaNueva];
                        foreach (var label in cajaObj.AsignacionLabels)
                        {
                            if (label.Text == empacador)
                            {
                                Console.WriteLine($"Resaltando: {empacador} en {cajaNueva}");
                                ResaltarLabel(label);
                                break;
                            }
                        }
                    }
                }
            }

            // Actualiza el diccionario de asignaciones anteriores para la próxima comparación
            empacadorCajaAnterior.Clear();
            foreach (var empacador in empacadorCajaActual.Keys)
            {
                empacadorCajaAnterior[empacador] = empacadorCajaActual[empacador];
            }
        }


        private void ProcesarListaDeEspera()
        {
            if (!empacadoresEnEspera.Any()) return;

            var openCajas = cajass.Values
                .Where(c => c.IsOpen && !c.IsOnBreak)
                .OrderByDescending(c => c.Empacadores.Count == 0) // Priorizar vacías
                .ThenBy(c => c.Empacadores.Count)
                .ToList();

            if (!openCajas.Any()) return;

            // Crear una lista para no modificar mientras iteramos
            var empacadoresAAsignar = new List<string>(empacadoresEnEspera);

            foreach (var emp in empacadoresAAsignar)
            {
                foreach (var caja in openCajas)
                {
                    if (caja.Empacadores.Count < 3)
                    {
                        caja.Empacadores.Add(emp);
                        caja.UpdateUI();
                        empacadoresEnEspera.Remove(emp);
                        RegistrarCambio($"{emp} asignado desde lista de espera a {caja.Nombre}");
                        break;
                    }
                }
            }

            // Actualizar la UI después de procesar
            RefrescarListaEspera();
            CheckAndHighlightReassignments();
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
            foreach (var empacador in empacadores)
            {
                if (!empacadoresEnEspera.Contains(empacador))
                {
                    empacadoresEnEspera.Add(empacador);
                    RegistrarCambio($"{empacador} agregado a la lista de espera");
                }
            }
            RefrescarListaEspera();
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

        private void BalancearEmpacadores()
        {
            var cajasAbiertas = cajass.Values
                .Where(c => c.IsOpen && !c.IsOnBreak)
                .OrderBy(c => c.Empacadores.Count)
                .ToList();

            if (cajasAbiertas.Count < 1) return;

            // Calcular la distribución ideal (máximo 2 por caja)
            int totalEmpacadores = cajasAbiertas.Sum(c => c.Empacadores.Count);
            int maxPorCaja = (int)Math.Ceiling((double)totalEmpacadores / cajasAbiertas.Count);

            // Redistribuir empacadores
            foreach (var caja in cajasAbiertas.Where(c => c.Empacadores.Count > maxPorCaja))
            {
                while (caja.Empacadores.Count > maxPorCaja)
                {
                    var empacador = caja.Empacadores.Last();
                    caja.Empacadores.Remove(empacador);

                    var cajaMenosOcupada = cajasAbiertas
                        .Where(c => c.Empacadores.Count < maxPorCaja)
                        .OrderBy(c => c.Empacadores.Count)
                        .FirstOrDefault();

                    if (cajaMenosOcupada != null)
                    {
                        cajaMenosOcupada.Empacadores.Add(empacador);
                        RegistrarCambio($"{empacador} movido de {caja.Nombre} a {cajaMenosOcupada.Nombre} para balancear");
                    }
                }
                caja.UpdateUI();
            }

            ResaltarReasignacionesExclusivas();
            CheckAndHighlightReassignments();
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

            SaveCurrentStateToUndoStack();

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

            // Primero procesar la lista de espera y luego balancear
            ProcesarListaDeEspera();
            BalancearEmpacadores();
            RefrescarListaEspera();
            ResaltarReasignacionesExclusivas();
           

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            string selectedCaja = comboBox2.SelectedItem.ToString();
            CambiarColorCaja(selectedCaja, Color.Transparent);
            MoverCaja(comboBox2, comboBox1, selectedCaja);
            CloseCaja(selectedCaja);
            RefrescarListaEspera();
           
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

            // Verificar si la caja está abierta
            if (!cajaCierre.IsOpen)
            {
                MessageBox.Show("La caja seleccionada ya está cerrada.");
                return;
            }

            // Preguntamos si el cierre será mayor a 45 minutos
            var resultt = MessageBox.Show("¿El cierre será mayor a 45 minutos?",
                                         "Cierre Temporal",
                                         MessageBoxButtons.YesNo);

            // 1) Marcamos la caja como cerrada temporalmente
            cajaCierre.IsOpen = false;
            CambiarColorCaja(selectedCaja, Color.Orange);

            // 2) Guardamos los empacadores que tenía en un diccionario
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

            // Mover la caja al comboBox correspondiente
            MoverCaja(comboBox3, comboBox1, selectedCaja);

            RegistrarCambio($"{DateTime.Now:HH:mm:ss} - La caja {selectedCaja} fue cerrada temporalmente.");
            foreach (var emp in temporalClosureEmpacadores[selectedCaja])
            {
                RegistrarCambio($"{DateTime.Now:HH:mm:ss} - El empacador {emp} fue reasignado debido al cierre temporal de la caja {selectedCaja}.");
            }

            // Actualizar la UI
            RefrescarListaEspera();
            CheckAndHighlightReassignments();
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

                // 10. Llamar al método para resaltar reasignaciones
                ResaltarReasignacionesExclusivas(); // ✅ Llamada al método

                // 11. Mover la caja original al ComboBox1
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
                SaveCurrentStateToUndoStack();

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

        private void RefrescarListaEspera()
        {
            if (LstEspera.InvokeRequired)
            {
                LstEspera.Invoke(new MethodInvoker(RefrescarListaEspera));
            }
            else
            {
                LstEspera.Items.Clear();
                foreach (var emp in empacadoresEnEspera)
                {
                    LstEspera.Items.Add(emp);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UndoLastOperation();
        }
    }


}
