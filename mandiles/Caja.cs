using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace mandiles
{
    internal class Caja
    {
        public string Nombre { get; set; }
        public Label MainLabel { get; set; }
        public List<Label> AsignacionLabels { get; set; }
        public List<string> Empacadores { get; set; } = new List<string>();
        public bool IsOpen { get; set; } = false;
        public bool IsOnBreak { get; set; } = false;


        public Caja(string nombre, Label mainLabel, List<Label> asignacionLabels)
        {
            Nombre = nombre;
            MainLabel = mainLabel;
            AsignacionLabels = asignacionLabels;
        }



        // Actualiza los labels de asignación de acuerdo a la lista de empacadores
        // Actualiza los labels de asignación de acuerdo a la lista de empacadores
        public void UpdateUI()
        {
            for (int i = 0; i < AsignacionLabels.Count; i++)
            {
                if (i < Empacadores.Count)
                {
                    string empacador = Empacadores[i];
                    Label label = AsignacionLabels[i];

                    // Guardar si ya estaba resaltado (por cualquier razón)
                    bool estaResaltado = Form1.highlightedLabels.ContainsKey(label);

                    // Actualizar texto del label
                    label.Text = empacador;
                    label.Visible = true;

                    // Mantener resaltado si:
                    // - Ya estaba resaltado
                    // - O si el empacador fue recientemente reasignado (según Form1.previousAssignments)
                    if (estaResaltado ||
                        Form1.highlightedLabels.Values.Any(t => (DateTime.Now - t).TotalSeconds < 60 &&
                                                              Form1.highlightedLabels.Keys.Any(l => l.Text == empacador)))
                    {
                        label.ForeColor = Color.Purple;
                        label.BorderStyle = BorderStyle.Fixed3D;
                        if (!Form1.highlightedLabels.ContainsKey(label))
                        {
                            Form1.highlightedLabels[label] = DateTime.Now; // Registrar para el temporizador
                        }
                    }
                    else
                    {
                        label.ForeColor = SystemColors.ControlText;
                        label.BorderStyle = BorderStyle.None;
                        if (Form1.highlightedLabels.ContainsKey(label))
                        {
                            Form1.highlightedLabels.Remove(label);
                        }
                    }
                }
                else
                {
                    AsignacionLabels[i].Text = "";
                    AsignacionLabels[i].Visible = false;
                }
            }
        }

        public bool TieneEspacio()
        {
            return Empacadores.Count < 3;  // Suponiendo que la caja tiene un máximo de 3 empacadores
        } 

    }
}
