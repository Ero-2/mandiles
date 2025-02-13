using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mandiles
{
    internal class Caja
    {
        public string Nombre { get; set; }
        public Label MainLabel { get; set; }
        public List<Label> AsignacionLabels { get; set; }
        public List<string> Empacadores { get; set; } = new List<string>();
        public bool IsOpen { get; set; } = false;

        public Caja(string nombre, Label mainLabel, List<Label> asignacionLabels)
        {
            Nombre = nombre;
            MainLabel = mainLabel;
            AsignacionLabels = asignacionLabels;
        }

        // Actualiza los labels de asignación de acuerdo a la lista de empacadores
        public void UpdateUI()
        {
            for (int i = 0; i < AsignacionLabels.Count; i++)
            {
                if (i < Empacadores.Count)
                {
                    AsignacionLabels[i].Text = Empacadores[i];
                    AsignacionLabels[i].Visible = true;
                }
                else
                {
                    AsignacionLabels[i].Text = "";
                    AsignacionLabels[i].Visible = false;
                }
            }
        }

    }
}
