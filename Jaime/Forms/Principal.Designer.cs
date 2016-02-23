namespace Jaime {
    partial class Principal {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principal));
            this.sysIcone = new System.Windows.Forms.NotifyIcon(this.components);
            this.pbIcone = new System.Windows.Forms.PictureBox();
            this.btnConfiguracoes = new System.Windows.Forms.Button();
            this.btnAjuda = new System.Windows.Forms.Button();
            this.txtPesquisar = new Jaime.Controls.TextBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcone)).BeginInit();
            this.SuspendLayout();
            // 
            // sysIcone
            // 
            this.sysIcone.Icon = ((System.Drawing.Icon)(resources.GetObject("sysIcone.Icon")));
            this.sysIcone.Text = "Jaime";
            this.sysIcone.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.sysIcon_MouseDoubleClick);
            // 
            // pbIcone
            // 
            this.pbIcone.Cursor = System.Windows.Forms.Cursors.NoMove2D;
            this.pbIcone.Image = global::Jaime.Properties.Resources.Jaime;
            this.pbIcone.Location = new System.Drawing.Point(0, 0);
            this.pbIcone.Name = "pbIcone";
            this.pbIcone.Size = new System.Drawing.Size(43, 43);
            this.pbIcone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcone.TabIndex = 4;
            this.pbIcone.TabStop = false;
            this.pbIcone.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbIcone_MouseDown);
            // 
            // btnConfiguracoes
            // 
            this.btnConfiguracoes.BackgroundImage = global::Jaime.Properties.Resources.Configuration;
            this.btnConfiguracoes.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnConfiguracoes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfiguracoes.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnConfiguracoes.FlatAppearance.BorderSize = 0;
            this.btnConfiguracoes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfiguracoes.Location = new System.Drawing.Point(655, 22);
            this.btnConfiguracoes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConfiguracoes.Name = "btnConfiguracoes";
            this.btnConfiguracoes.Size = new System.Drawing.Size(22, 20);
            this.btnConfiguracoes.TabIndex = 3;
            this.btnConfiguracoes.UseVisualStyleBackColor = true;
            this.btnConfiguracoes.Click += new System.EventHandler(this.btnConfiguracoes_Click);
            // 
            // btnAjuda
            // 
            this.btnAjuda.BackColor = System.Drawing.SystemColors.Control;
            this.btnAjuda.BackgroundImage = global::Jaime.Properties.Resources.Help;
            this.btnAjuda.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAjuda.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAjuda.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnAjuda.FlatAppearance.BorderSize = 0;
            this.btnAjuda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAjuda.Location = new System.Drawing.Point(655, 1);
            this.btnAjuda.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAjuda.Name = "btnAjuda";
            this.btnAjuda.Size = new System.Drawing.Size(22, 20);
            this.btnAjuda.TabIndex = 2;
            this.btnAjuda.UseVisualStyleBackColor = false;
            this.btnAjuda.Click += new System.EventHandler(this.btnAjuda_Click);
            // 
            // txtPesquisar
            // 
            this.txtPesquisar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPesquisar.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPesquisar.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPesquisar.Location = new System.Drawing.Point(45, 3);
            this.txtPesquisar.Name = "txtPesquisar";
            this.txtPesquisar.Size = new System.Drawing.Size(608, 37);
            this.txtPesquisar.TabIndex = 5;
            this.txtPesquisar.TextChanged += new System.EventHandler(this.Pesquisar);
            this.txtPesquisar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPesquisar_KeyDown);
            this.txtPesquisar.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPesquisar_KeyPress);
            this.txtPesquisar.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPesquisar_KeyUp);
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 43);
            this.Controls.Add(this.txtPesquisar);
            this.Controls.Add(this.pbIcone);
            this.Controls.Add(this.btnConfiguracoes);
            this.Controls.Add(this.btnAjuda);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Principal";
            this.Opacity = 0.8D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Jaime: O que deseja Sr.?";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Principal_FormClosing);
            this.Load += new System.EventHandler(this.Principal_Load);
            this.Shown += new System.EventHandler(this.Principal_Shown);
            this.VisibleChanged += new System.EventHandler(this.Principal_VisibleChanged);
            this.Move += new System.EventHandler(this.Principal_Move);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcone)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon sysIcone;
        private System.Windows.Forms.Button btnAjuda;
        private System.Windows.Forms.Button btnConfiguracoes;
        private System.Windows.Forms.PictureBox pbIcone;
        private Controls.TextBoxControl txtPesquisar;
    }
}