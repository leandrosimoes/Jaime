namespace Jaime {
    partial class ResultadosPesquisa {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultadosPesquisa));
            this.lbResultadosPesquisas = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lbResultadosPesquisas
            // 
            this.lbResultadosPesquisas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbResultadosPesquisas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbResultadosPesquisas.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbResultadosPesquisas.FormattingEnabled = true;
            this.lbResultadosPesquisas.ItemHeight = 16;
            this.lbResultadosPesquisas.Location = new System.Drawing.Point(0, -1);
            this.lbResultadosPesquisas.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lbResultadosPesquisas.Name = "lbResultadosPesquisas";
            this.lbResultadosPesquisas.Size = new System.Drawing.Size(648, 128);
            this.lbResultadosPesquisas.TabIndex = 0;
            this.lbResultadosPesquisas.Click += new System.EventHandler(this.lbResultadosPesquisas_Click);
            this.lbResultadosPesquisas.SelectedValueChanged += new System.EventHandler(this.lbResultadosPesquisas_SelectedValueChanged);
            this.lbResultadosPesquisas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbResultadosPesquisas_KeyDown);
            // 
            // ResultadosPesquisa
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 129);
            this.Controls.Add(this.lbResultadosPesquisas);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ResultadosPesquisa";
            this.ShowInTaskbar = false;
            this.Shown += new System.EventHandler(this.ResultadosPesquisa_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbResultadosPesquisas;
    }
}