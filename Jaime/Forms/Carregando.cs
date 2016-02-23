using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Jaime.Properties;
using Jaime.Repository;

namespace Jaime {
    public partial class Carregando : Form {
        #region Variaveis para drag and drop do form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private readonly JaimeRepository _repository = new JaimeRepository();
        #endregion

        public Carregando() {
            InitializeComponent();
            lblStatus.Show();
            pbIcone.Show();
        }

        private void Carregando_VisibleChanged(object sender, EventArgs e) {
            Opacity = Double.Parse(_repository.ObterConfiguracoes().First().Opacidade.ToString());
        }

        delegate void StringParameterDelegate(string Text);
        delegate void SplashShowCloseDelegate();

        bool _closeSplashScreenFlag = false;

        public void MostarSplashCarregando() {
            if (InvokeRequired) {
                BeginInvoke(new SplashShowCloseDelegate(MostarSplashCarregando));
                return;
            }

            lblVersion.Text = Settings.Default.Versao;

            Show();
            Application.Run(this);
        }

        public void FecharSplashCarregando() {
            if (InvokeRequired) {
                BeginInvoke(new SplashShowCloseDelegate(FecharSplashCarregando));
                return;
            }
            _closeSplashScreenFlag = true;
            Close();
        }

        public void MudarMensagemStatus(string Text) {
            if (InvokeRequired) {
                BeginInvoke(new StringParameterDelegate(MudarMensagemStatus), new object[] { Text });
                return;
            }

            lblStatus.Text = Text;
        }

        private void Carregando_FormClosing_1(object sender, FormClosingEventArgs e) {
            if (_closeSplashScreenFlag == false)
                e.Cancel = true;
        }

        private void Carregando_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void label1_Click(object sender, EventArgs e) {
            Process.Start("https://lesimoes.com.br");
        }
    }
}
