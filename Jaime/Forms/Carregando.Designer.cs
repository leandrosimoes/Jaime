namespace Jaime {
    partial class Carregando {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Carregando));
            this.pbIcone = new System.Windows.Forms.PictureBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pbIcone
            // 
            this.pbIcone.Image = ((System.Drawing.Image)(resources.GetObject("pbIcone.Image")));
            this.pbIcone.Location = new System.Drawing.Point(48, 333);
            this.pbIcone.Name = "pbIcone";
            this.pbIcone.Size = new System.Drawing.Size(340, 27);
            this.pbIcone.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcone.TabIndex = 0;
            this.pbIcone.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblStatus.Location = new System.Drawing.Point(12, 368);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(413, 19);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Jaime.Properties.Resources.Splash;
            this.pictureBox1.Location = new System.Drawing.Point(-10, -131);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(454, 458);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            // 
            // lblVersion
            // 
            this.lblVersion.BackColor = System.Drawing.Color.Transparent;
            this.lblVersion.ForeColor = System.Drawing.Color.Black;
            this.lblVersion.Location = new System.Drawing.Point(143, 298);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(282, 19);
            this.lblVersion.TabIndex = 3;
            this.lblVersion.Text = "lblVersion";
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(325, 397);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "by Leandro Simões";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Carregando
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(437, 420);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pbIcone);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.SteelBlue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Carregando";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Abrindo Jaime";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Carregando_FormClosing_1);
            this.VisibleChanged += new System.EventHandler(this.Carregando_VisibleChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Carregando_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbIcone;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label1;
    }
}