using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using ClosedXML.Excel;
using System.IO;


namespace mandiles
{
    public partial class Form2 : Form
    {
        //private string connectionString = "Data Source=C:\\Users\\eroer\\Downloads\\horariosEmpacadores.db;Version=3;";
        private Empacadores EmpacadoresHorario;
        private Form1 form1;

        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            form1 = mainForm;
            EmpacadoresHorario = new Empacadores();

            // IMPORTANTE: Desuscribir primero para evitar suscripciones múltiples
            dataGridView1.CellEndEdit -= dataGridView1_CellEndEdit;
            dataGridView1.UserDeletingRow -= dataGridView1_UserDeletingRow;
            clbAusencias.ItemCheck -= clbAusencias_ItemCheck;  // Desuscribir primero
            dataGridView1.RowEnter -= dataGridView1_RowEnter;

            // Ahora suscribir los eventos
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            dataGridView1.UserDeletingRow += dataGridView1_UserDeletingRow;
            clbAusencias.ItemCheck += clbAusencias_ItemCheck;  // Suscribir una sola vez
            dataGridView1.RowEnter += dataGridView1_RowEnter;

            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                // Ruta relativa: el archivo debe estar junto al .exe
                string rutaExcel = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HorarioEmpacadores.xlsx");

                var workbook = new XLWorkbook(rutaExcel);
                var worksheet = workbook.Worksheet("HorarioEmpacadores");

                EmpacadoresHorario = new Empacadores();

                var dataTable = new System.Data.DataTable();
                bool encabezadosLeidos = false;

                foreach (var fila in worksheet.RowsUsed())
                {
                    if (!encabezadosLeidos)
                    {
                        foreach (var celda in fila.Cells())
                        {
                            dataTable.Columns.Add(celda.GetString());
                        }
                        encabezadosLeidos = true;
                    }
                    else
                    {
                        var newRow = dataTable.NewRow();
                        int colIndex = 0;
                        string nombre = fila.Cell(1).GetString();

                        foreach (var celda in fila.Cells(1, 8))
                        {
                            newRow[colIndex++] = celda.GetString();
                        }

                        EmpacadoresHorario.AsignarHorario(nombre, "LUNES", fila.Cell(2).GetString());
                        EmpacadoresHorario.AsignarHorario(nombre, "MARTES", fila.Cell(3).GetString());
                        EmpacadoresHorario.AsignarHorario(nombre, "MIERCOLES", fila.Cell(4).GetString());
                        EmpacadoresHorario.AsignarHorario(nombre, "JUEVES", fila.Cell(5).GetString());
                        EmpacadoresHorario.AsignarHorario(nombre, "VIERNES", fila.Cell(6).GetString());
                        EmpacadoresHorario.AsignarHorario(nombre, "SABADO", fila.Cell(7).GetString());
                        EmpacadoresHorario.AsignarHorario(nombre, "DOMINGO", fila.Cell(8).GetString());

                        dataTable.Rows.Add(newRow);
                    }
                }

                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos desde Excel: " + ex.Message);
            }
        }

        private void GuardarDatos()
        {
            try
            {
                // Mismo archivo Excel, junto al .exe
                string rutaExcel = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HorarioEmpacadores.xlsx");

                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("HorarioEmpacadores");

                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                {
                    worksheet.Cell(1, i + 1).Value = dataGridView1.Columns[i].HeaderText;
                }

                for (int fila = 0; fila < dataGridView1.Rows.Count; fila++)
                {
                    for (int col = 0; col < dataGridView1.Columns.Count; col++)
                    {
                        var valor = dataGridView1.Rows[fila].Cells[col].Value;
                        worksheet.Cell(fila + 2, col + 1).Value = valor != null ? valor.ToString() : "";
                    }
                }

                workbook.SaveAs(rutaExcel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar los datos en Excel: " + ex.Message);
            }
        }





        private void button1_Click_1(object sender, EventArgs e)
        {
            Form1 form1 = Application.OpenForms["Form1"] as Form1;
            if (form1 == null)
            {
                MessageBox.Show("No se encontró el Form1.");
                return;
            }

            List<Label> cajasAbiertas = form1.ObtenerCajasAbiertas();
            if (cajasAbiertas.Count == 0)
            {
                MessageBox.Show("No hay cajas abiertas para asignar.");
                return;
            }

            string diaActual = DateTime.Now.ToString("dddd", new CultureInfo("es-ES"))
                .Replace("á", "a").Replace("é", "e").Replace("í", "i")
                .Replace("ó", "o").Replace("ú", "u").ToUpper();

            var horariosDict = EmpacadoresHorario.ObtenerHorario();
            List<string> empacadoresHoy = horariosDict
                .Where(emp => emp.Value.ContainsKey(diaActual)
                              && !string.IsNullOrWhiteSpace(emp.Value[diaActual]) // No vacío
                              && emp.Value[diaActual].ToUpper() != "DESCANSO") // No en descanso
                .Select(emp => emp.Key).ToList();

            if (empacadoresHoy.Count == 0)
            {
                MessageBox.Show($"No hay empacadores programados para {diaActual}.");
                return;
            }

            // Obtener los empacadores ausentes
            List<string> empacadoresAusentes = new List<string>();
            foreach (var item in clbAusencias.CheckedItems)
            {
                empacadoresAusentes.Add(item.ToString());
            }

            // Excluir a los empacadores ausentes
            empacadoresHoy = empacadoresHoy.Except(empacadoresAusentes).ToList();

            // Si no hay empacadores disponibles para asignar
            if (empacadoresHoy.Count == 0)
            {
                MessageBox.Show($"No hay empacadores disponibles para asignar en {diaActual}.");
                return;
            }

            Random rng = new Random();
            List<string> empacadoresMezclados = empacadoresHoy.OrderBy(x => rng.Next()).ToList();
            List<Label> cajasBarajadas = cajasAbiertas.OrderBy(x => rng.Next()).ToList();

            int baseCount = empacadoresMezclados.Count / cajasBarajadas.Count;
            int remainder = empacadoresMezclados.Count % cajasBarajadas.Count;

            Dictionary<Label, List<string>> asignaciones = new Dictionary<Label, List<string>>();
            foreach (var caja in cajasBarajadas)
            {
                asignaciones[caja] = new List<string>();
            }

            int currentIndex = 0;
            foreach (var caja in cajasBarajadas)
            {
                for (int i = 0; i < baseCount; i++)
                {
                    if (currentIndex < empacadoresMezclados.Count)
                    {
                        asignaciones[caja].Add(empacadoresMezclados[currentIndex++]);
                    }
                }
            }

            for (int i = 0; i < remainder; i++)
            {
                if (currentIndex < empacadoresMezclados.Count)
                {
                    asignaciones[cajasBarajadas[i]].Add(empacadoresMezclados[currentIndex++]);
                }
            }

            form1.ActualizarAsignaciones(asignaciones);

            string mensaje = $"Asignación de empacadores para {diaActual}:\n\n" +
                string.Join("\n", asignaciones.Select(kvp => $"{kvp.Key.Text}: {string.Join(", ", kvp.Value)}"));

            MessageBox.Show(mensaje, "Asignación de Empacadores");
        }


        private void MostrarEmpacadoresDelDia()
        {
            DateTime fechaSeleccionada = dateTimePicker1.Value; // Fecha seleccionada en el DateTimePicker
            List<string> empacadores = EmpacadoresHorario.ObtenerEmpacadoresDelHorario(fechaSeleccionada);

            lbAsignadosHoy.Items.Clear(); // Limpiar la lista antes de agregar los nuevos datos
            lbAsignadosHoy.Items.AddRange(empacadores.ToArray()); // Agregar los empacadores a la lista

            clbAusencias.Items.Clear(); // Limpiar la lista antes de agregar los nuevos datos
            clbAusencias.Items.AddRange(empacadores.ToArray()); // Agregar los empacadores a la lista

            for (int i = 0; i < clbAusencias.Items.Count; i++)
            {
                clbAusencias.SetItemChecked(i, false); // Asegurarse de que no estén seleccionados al inicio
            }

        }


        private void Form2_Load(object sender, EventArgs e)
        {
            MostrarEmpacadoresDelDia();
            CargarDatos();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            MostrarEmpacadoresDelDia();
        }

       

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

            GuardarDatos(); 
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta fila?", "Confirmar eliminación", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
        }

       /*private void EliminarRegistro(string empacador)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    // Abrir la conexión
                    conn.Open();

                    string query = "DELETE FROM HorarioEmpacadores WHERE Empacador = @Empacador";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        // Añadir el parámetro para la consulta SQL
                        cmd.Parameters.AddWithValue("@Empacador", empacador);

                        // Ejecutar la consulta
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Registro eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            // Ahora elimina también la fila de la DataGridView
                            dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo encontrar el registro en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el registro: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/


       /* private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)  // Verificar si hay una fila seleccionada
            {
                string empacador = dataGridView1.SelectedRows[0].Cells["Empacador"].Value?.ToString();

                if (!string.IsNullOrEmpty(empacador))
                {
                    DialogResult confirmacion = MessageBox.Show(
                        $"¿Estás seguro de eliminar a {empacador}?",
                        "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (confirmacion == DialogResult.Yes)
                    {
                        EliminarRegistro(empacador);
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un registro antes de eliminar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }*/

      

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
        

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }





        // Variable a nivel de clase para controlar el procesamiento
        private bool procesandoCheck = false;

        private void clbAusencias_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Si ya estamos procesando, cancelamos esta llamada
            if (procesandoCheck)
            {
                e.NewValue = e.CurrentValue; // Mantener el valor actual
                return;
            }

            if (e.NewValue == CheckState.Unchecked)
            {
                try
                {
                    // Activamos la bandera
                    procesandoCheck = true;

                    // Suspendemos temporalmente la actualización de la interfaz
                    clbAusencias.BeginUpdate();

                    string empacador = clbAusencias.Items[e.Index].ToString();

                    DialogResult respuesta = MessageBox.Show(
                        $"¿Estás seguro de incluir a {empacador} en las asignaciones?",
                        "Confirmar inclusión",
                        MessageBoxButtons.YesNo);

                    if (respuesta == DialogResult.Yes)
                    {
                        Form1 form1 = Application.OpenForms["Form1"] as Form1;
                        form1?.AgregarEmpacadorACajasDisponibles(empacador);
                    }
                    else
                    {
                        // Cancelamos el cambio programáticamente
                        e.NewValue = CheckState.Checked;
                    }
                }
                finally
                {
                    // Reanudamos la actualización de la interfaz
                    clbAusencias.EndUpdate();

                    // IMPORTANTE: Siempre desactivamos la bandera, incluso si hay errores
                    procesandoCheck = false;
                }
            }
        }

        private string _nombreOriginal;
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && !dataGridView1.Rows[e.RowIndex].IsNewRow)
            {
                _nombreOriginal = dataGridView1.Rows[e.RowIndex].Cells["EMPACADOR"].Value?.ToString();
            }
        }

        private void clbAusencias_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

}













