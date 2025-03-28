﻿using System;
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

namespace mandiles
{
    public partial class Form2 : Form
    {
        private string connectionString = "Data Source=C:\\Users\\eroer\\Downloads\\horariosEmpacadores.db;Version=3;";
        private Empacadores EmpacadoresHorario;
        private Form1 form1;

        public Form2(Form1 mainForm)
        {
            InitializeComponent();
            form1 = mainForm;
            EmpacadoresHorario = new Empacadores();
            CargarDatos();

            dataGridView1.RowValidated += dataGridView1_RowValidated;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            dataGridView1.UserDeletingRow += dataGridView1_UserDeletingRow;
            clbAusencias.ItemCheck += clbAusencias_ItemCheck;
            dataGridView1.RowEnter += dataGridView1_RowEnter;
        }

        private void CargarDatos()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();

                    // Cargar ListBox
                    string query = "SELECT EMPACADOR FROM HorarioEmpacadores";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        
                    }

                    // Cargar Horarios en EmpacadoresHorario
                    query = "SELECT Empacador, Lunes, Martes, Miercoles, Jueves, Viernes, Sabado, Domingo FROM HorarioEmpacadores";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nombre = reader["Empacador"].ToString();
                            EmpacadoresHorario.AsignarHorario(nombre, "LUNES", reader["Lunes"].ToString());
                            EmpacadoresHorario.AsignarHorario(nombre, "MARTES", reader["Martes"].ToString());
                            EmpacadoresHorario.AsignarHorario(nombre, "MIERCOLES", reader["Miercoles"].ToString());
                            EmpacadoresHorario.AsignarHorario(nombre, "JUEVES", reader["Jueves"].ToString());
                            EmpacadoresHorario.AsignarHorario(nombre, "VIERNES", reader["Viernes"].ToString());
                            EmpacadoresHorario.AsignarHorario(nombre, "SABADO", reader["Sabado"].ToString());
                            EmpacadoresHorario.AsignarHorario(nombre, "DOMINGO", reader["Domingo"].ToString());
                        }
                    }

                    // Cargar DataGridView
                    query = "SELECT * FROM HorarioEmpacadores";
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, conn);
                    System.Data.DataTable dt = new System.Data.DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
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

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].IsNewRow) return;

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string nuevoNombre = dataGridView1.Rows[e.RowIndex].Cells["Empacador"].Value?.ToString();
                if (string.IsNullOrEmpty(nuevoNombre)) return;

                // Verificar si el nombre fue modificado
                if (nuevoNombre != _nombreOriginal)
                {
                    // 1. Comprobar si el nuevo nombre ya existe
                    string checkQuery = "SELECT COUNT(*) FROM HorarioEmpacadores WHERE Empacador = @NuevoNombre";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@NuevoNombre", nuevoNombre);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            MessageBox.Show("¡El nombre ya existe!");
                            dataGridView1.Rows[e.RowIndex].Cells["Empacador"].Value = _nombreOriginal; // Revertir
                            return;
                        }
                    }

                    // 2. Actualizar el registro existente con el nuevo nombre
                    string updateQuery = "UPDATE HorarioEmpacadores SET " +
                                        "Empacador = @NuevoNombre, Lunes = @Lunes, Martes = @Martes, " +
                                        "Miercoles = @Miercoles, Jueves = @Jueves, Viernes = @Viernes, " +
                                        "Sabado = @Sabado, Domingo = @Domingo " +
                                        "WHERE Empacador = @NombreOriginal";

                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@NuevoNombre", nuevoNombre);
                        cmd.Parameters.AddWithValue("@NombreOriginal", _nombreOriginal);
                        cmd.Parameters.AddWithValue("@Lunes", dataGridView1.Rows[e.RowIndex].Cells["Lunes"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Martes", dataGridView1.Rows[e.RowIndex].Cells["Martes"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Miercoles", dataGridView1.Rows[e.RowIndex].Cells["Miercoles"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Jueves", dataGridView1.Rows[e.RowIndex].Cells["Jueves"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Viernes", dataGridView1.Rows[e.RowIndex].Cells["Viernes"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Sabado", dataGridView1.Rows[e.RowIndex].Cells["Sabado"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Domingo", dataGridView1.Rows[e.RowIndex].Cells["Domingo"].Value ?? "DESCANSO");

                        cmd.ExecuteNonQuery();
                    }

                    _nombreOriginal = nuevoNombre; // Actualizar el nombre original
                }
                else
                {
                    // Si el nombre no cambió, solo actualizar los horarios
                    string updateQuery = "UPDATE HorarioEmpacadores SET " +
                                        "Lunes = @Lunes, Martes = @Martes, Miercoles = @Miercoles, " +
                                        "Jueves = @Jueves, Viernes = @Viernes, Sabado = @Sabado, Domingo = @Domingo " +
                                        "WHERE Empacador = @Empacador";

                    using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Empacador", nuevoNombre);
                        cmd.Parameters.AddWithValue("@Lunes", dataGridView1.Rows[e.RowIndex].Cells["Lunes"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Martes", dataGridView1.Rows[e.RowIndex].Cells["Martes"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Miercoles", dataGridView1.Rows[e.RowIndex].Cells["Miercoles"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Jueves", dataGridView1.Rows[e.RowIndex].Cells["Jueves"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Viernes", dataGridView1.Rows[e.RowIndex].Cells["Viernes"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Sabado", dataGridView1.Rows[e.RowIndex].Cells["Sabado"].Value ?? "DESCANSO");
                        cmd.Parameters.AddWithValue("@Domingo", dataGridView1.Rows[e.RowIndex].Cells["Domingo"].Value ?? "DESCANSO");

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE HorarioEmpacadores SET " +
                               dataGridView1.Columns[e.ColumnIndex].Name + " = @valor " +
                               "WHERE Empacador = @Empacador";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@valor", dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                    cmd.Parameters.AddWithValue("@Empacador", dataGridView1.Rows[e.RowIndex].Cells["Empacador"].Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Estás seguro de que deseas eliminar este registro?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM HorarioEmpacadores WHERE Empacador = @Empacador";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Empacador", e.Row.Cells["Empacador"].Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void EliminarRegistro(string empacador)
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
        }


        private void button2_Click(object sender, EventArgs e)
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
        }

      

        private void BtnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

      
        

        private void BtnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }

      

        

        private void clbAusencias_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                string empacador = clbAusencias.Items[e.Index].ToString();

                // Deshabilitamos temporalmente el evento para evitar múltiples ejecuciones
                clbAusencias.ItemCheck -= clbAusencias_ItemCheck;

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
                    // Restauramos el estado a "Checked" sin disparar nuevamente el evento
                    clbAusencias.SetItemCheckState(e.Index, CheckState.Checked);
                }

                // Rehabilitamos el evento
                clbAusencias.ItemCheck += clbAusencias_ItemCheck;
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
    }

}













