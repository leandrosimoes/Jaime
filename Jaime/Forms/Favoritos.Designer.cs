namespace Jaime {
    partial class Favoritos {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Favoritos));
            this.gridFavoritos = new System.Windows.Forms.DataGridView();
            this.Selecionar = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Favorito = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Caminho = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnExcluirFavoritos = new System.Windows.Forms.Button();
            this.txtPesquisar = new System.Windows.Forms.TextBox();
            this.lblQtdCaracteres = new System.Windows.Forms.Label();
            this.btnSelecao = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridFavoritos)).BeginInit();
            this.SuspendLayout();
            // 
            // gridFavoritos
            // 
            this.gridFavoritos.AllowUserToAddRows = false;
            this.gridFavoritos.AllowUserToDeleteRows = false;
            this.gridFavoritos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridFavoritos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFavoritos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Selecionar,
            this.Favorito,
            this.Caminho});
            this.gridFavoritos.Location = new System.Drawing.Point(12, 55);
            this.gridFavoritos.Name = "gridFavoritos";
            this.gridFavoritos.Size = new System.Drawing.Size(547, 248);
            this.gridFavoritos.TabIndex = 0;
            // 
            // Selecionar
            // 
            this.Selecionar.HeaderText = "Selecionar";
            this.Selecionar.Name = "Selecionar";
            this.Selecionar.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Selecionar.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Favorito
            // 
            this.Favorito.HeaderText = "Favorito";
            this.Favorito.MinimumWidth = 100;
            this.Favorito.Name = "Favorito";
            this.Favorito.ReadOnly = true;
            // 
            // Caminho
            // 
            this.Caminho.HeaderText = "Caminho";
            this.Caminho.Name = "Caminho";
            this.Caminho.ReadOnly = true;
            // 
            // btnExcluirFavoritos
            // 
            this.btnExcluirFavoritos.Location = new System.Drawing.Point(484, 319);
            this.btnExcluirFavoritos.Name = "btnExcluirFavoritos";
            this.btnExcluirFavoritos.Size = new System.Drawing.Size(75, 23);
            this.btnExcluirFavoritos.TabIndex = 1;
            this.btnExcluirFavoritos.Text = "Excluir";
            this.btnExcluirFavoritos.UseVisualStyleBackColor = true;
            this.btnExcluirFavoritos.Click += new System.EventHandler(this.btnExcluirFavoritos_Click);
            // 
            // txtPesquisar
            // 
            this.txtPesquisar.Location = new System.Drawing.Point(12, 12);
            this.txtPesquisar.Name = "txtPesquisar";
            this.txtPesquisar.Size = new System.Drawing.Size(547, 20);
            this.txtPesquisar.TabIndex = 2;
            this.txtPesquisar.TextChanged += new System.EventHandler(this.txtPesquisar_TextChanged);
            // 
            // lblQtdCaracteres
            // 
            this.lblQtdCaracteres.AutoSize = true;
            this.lblQtdCaracteres.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQtdCaracteres.Location = new System.Drawing.Point(12, 35);
            this.lblQtdCaracteres.Name = "lblQtdCaracteres";
            this.lblQtdCaracteres.Size = new System.Drawing.Size(35, 14);
            this.lblQtdCaracteres.TabIndex = 4;
            this.lblQtdCaracteres.Text = "label2";
            // 
            // btnSelecao
            // 
            this.btnSelecao.Location = new System.Drawing.Point(12, 309);
            this.btnSelecao.Name = "btnSelecao";
            this.btnSelecao.Size = new System.Drawing.Size(94, 23);
            this.btnSelecao.TabIndex = 5;
            this.btnSelecao.Text = "Inverter seleção";
            this.btnSelecao.UseVisualStyleBackColor = true;
            this.btnSelecao.Click += new System.EventHandler(this.btnSelecao_Click);
            // 
            // Favoritos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 348);
            this.Controls.Add(this.btnSelecao);
            this.Controls.Add(this.lblQtdCaracteres);
            this.Controls.Add(this.txtPesquisar);
            this.Controls.Add(this.btnExcluirFavoritos);
            this.Controls.Add(this.gridFavoritos);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Favoritos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Favoritos";
            this.Load += new System.EventHandler(this.Favoritos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridFavoritos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gridFavoritos;
        private System.Windows.Forms.Button btnExcluirFavoritos;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selecionar;
        private System.Windows.Forms.DataGridViewTextBoxColumn Favorito;
        private System.Windows.Forms.DataGridViewTextBoxColumn Caminho;
        private System.Windows.Forms.TextBox txtPesquisar;
        private System.Windows.Forms.Label lblQtdCaracteres;
        private System.Windows.Forms.Button btnSelecao;
    }
}