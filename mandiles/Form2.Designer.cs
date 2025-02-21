namespace mandiles
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AgregarEmpacador = new System.Windows.Forms.Button();
            this.lbAsignadosHoy = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.clbAusencias = new System.Windows.Forms.CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Haettenschweiler", 26.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "MANDILES";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Haettenschweiler", 26.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(374, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(292, 37);
            this.label2.TabIndex = 2;
            this.label2.Text = "HORARIO DE EMPACADORES";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(19, 72);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(263, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Play Pretend", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(175, 11);
            this.label3.TabIndex = 4;
            this.label3.Text = "Nombre del empacador";
            // 
            // AgregarEmpacador
            // 
            this.AgregarEmpacador.Font = new System.Drawing.Font("Daydream", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AgregarEmpacador.Location = new System.Drawing.Point(19, 98);
            this.AgregarEmpacador.Name = "AgregarEmpacador";
            this.AgregarEmpacador.Size = new System.Drawing.Size(263, 26);
            this.AgregarEmpacador.TabIndex = 5;
            this.AgregarEmpacador.Text = "AGREGAR EMPACADOR";
            this.AgregarEmpacador.UseVisualStyleBackColor = true;
            this.AgregarEmpacador.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbAsignadosHoy
            // 
            this.lbAsignadosHoy.FormattingEnabled = true;
            this.lbAsignadosHoy.Location = new System.Drawing.Point(12, 130);
            this.lbAsignadosHoy.Name = "lbAsignadosHoy";
            this.lbAsignadosHoy.Size = new System.Drawing.Size(276, 134);
            this.lbAsignadosHoy.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Daydream", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(354, 351);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(362, 27);
            this.button1.TabIndex = 7;
            this.button1.Text = "ASIGNAR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(312, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(440, 296);
            this.dataGridView1.TabIndex = 8;
            // 
            // clbAusencias
            // 
            this.clbAusencias.FormattingEnabled = true;
            this.clbAusencias.Location = new System.Drawing.Point(19, 284);
            this.clbAusencias.Name = "clbAusencias";
            this.clbAusencias.Size = new System.Drawing.Size(120, 94);
            this.clbAusencias.TabIndex = 9;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.clbAusencias);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lbAsignadosHoy);
            this.Controls.Add(this.AgregarEmpacador);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button AgregarEmpacador;
        private System.Windows.Forms.ListBox lbAsignadosHoy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.CheckedListBox clbAusencias;
    }
}