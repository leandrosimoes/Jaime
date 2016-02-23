using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Jaime.Extensions;
using Jaime.Helpers;
using Jaime.Models;
using Jaime.Repository;
using Microsoft.Win32;

namespace Jaime {
    public partial class Configuracoes : Form {
        private readonly Principal _formPrincipal;
        private readonly ResultadosPesquisa _formResultadosPesquisa;
        private readonly JaimeRepository _repository = new JaimeRepository();
        private ConfiguracoesDirtyTracker _frmDirtyTracker;
        private bool _salvou;

        public Configuracoes(Principal formPrincipal, ResultadosPesquisa formResultadosPesquisa) {
            InitializeComponent();
            _formPrincipal = formPrincipal;
            _formResultadosPesquisa = formResultadosPesquisa;
        }

        private void Configuracoes_Load(object sender, EventArgs e) {
            SetupTooltips();
            CarregarValores();

            _salvou = false;
            _frmDirtyTracker = new ConfiguracoesDirtyTracker(this);
            _frmDirtyTracker.SetAsClean();
        }

        private void SetupTooltips() {
            var toolTip = new ToolTip();

            toolTip.AutoPopDelay = 20000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;
            toolTip.IsBalloon = true;

            toolTip.ShowAlways = true;

            abas.ShowToolTips = true;
            toolTip.SetToolTip(lblTooltipSubstituirWinR, "Ao pressionar as teclas \"Win+R\" o sistema irá abrir o Jaime já com o cursor preparado para executar um comando.");
            toolTip.SetToolTip(lblTooltipFavoritosNavegadores, "Adiciona à pesquisa os favoritos adicionados aos navegadores Chrome e Firefox.");
            toolTip.SetToolTip(tooltipUrlBuscas, "A Url deve ser montada com parâmetros entre chaves. " +
                Environment.NewLine +
                "O parametro padrão para a busca é {term}, o restante deve ser adicionado com numeração em ordem. Ex:" +
                Environment.NewLine +
                "url.com.br/{0}/{1}/{term}"
            );
            toolTip.SetToolTip(tooltipParametrosBuscas, "Os parâmetros devem ser adicionados separados por ponto e vírgula. Ex:" +
                Environment.NewLine +
                "parametro 0; parametro 1; parametro 2"
            );
        }

        private void CarregarValores() {
            var configuracoes = _repository.ObterConfiguracoes().First();
            var fontCollection = new InstalledFontCollection();

            foreach (var font in fontCollection.Families) {
                cmbFont.Items.Add(font.Name);
            }

            cmbFont.SelectedItem = configuracoes.Fonte;

            cbFavoritosChrome.Checked = configuracoes.FavoritosNavegadores;
            cbIniciar.Checked = configuracoes.IniciarComWindows;
            cbSubstituirWinR.Checked = configuracoes.SubstituirTeclaWindowsR;
            cbProxy.Checked = configuracoes.Proxy;

            txtCorFundo.BackColor = JaimeHelper.ObterCor(configuracoes.CorFundo);
            txtCorFonte.BackColor = JaimeHelper.ObterCor(configuracoes.CorFonte);

            CarregarConfiguracoesProxy();
            CarregarMotoresBuscas();

            numOpacidade.Value = CarregarOpacidade();
        }

        private void CarregarConfiguracoesProxy() {
            var configuracoes = _repository.ObterConfiguracoes().First();
            if (configuracoes.Proxy) {
                if (string.IsNullOrEmpty(configuracoes.ProxyServidor)) { return; }

                txtServidor.Text = configuracoes.ProxyServidor;
                txtPorta.Text = configuracoes.ProxyPorta;

                if (!string.IsNullOrEmpty(configuracoes.ProxyUsuario))
                    txtUsuario.Text = configuracoes.ProxyUsuario;

                if (!string.IsNullOrEmpty(configuracoes.ProxySenha))
                    txtSenha.Text = configuracoes.ProxySenha;
            }
        }

        private void CarregarMotoresBuscas() {
            var motores = _repository.ObterMotoresBuscas();
            int row = 0;

            gridBuscas.Rows.Clear();

            foreach (var motorBusca in motores) {
                gridBuscas.Rows.Insert(row, false, motorBusca.Id, motorBusca.Url, motorBusca.Comando, motorBusca.Parametros);
                row++;
            }
        }

        private void Salvar() {
            try {
                if (!_frmDirtyTracker.IsDirty) {
                    Close();
                    return;
                }

                DesabilitarCampos();

                var splashthread = new Thread(SplashCarregando.MostrarSplashCarregando) { IsBackground = true };
                splashthread.Start();

                SplashCarregando.MudarMensagemStatus("", 100);
                SplashCarregando.MudarMensagemStatus("Salvando configurações.", 2000);

                if (!ValidarCampos()) {
                    SplashCarregando.FecharSplashCarregando();
                    HabilitarCampos();
                    return;
                }

                var configuracoes = _repository.ObterConfiguracoes().First();
                configuracoes.FavoritosNavegadores = cbFavoritosChrome.Checked;
                configuracoes.Opacidade = SalvarOpacidade();
                configuracoes.IniciarComWindows = SetStartup();
                configuracoes.CorFundo = SalvarCor(txtCorFundo.BackColor);
                configuracoes.CorFonte = SalvarCor(txtCorFonte.BackColor);
                configuracoes.SubstituirTeclaWindowsR = cbSubstituirWinR.Checked;
                configuracoes.Proxy = cbProxy.Checked;
                configuracoes.ProxyServidor = txtServidor.Text;
                configuracoes.ProxyPorta = txtPorta.Text;
                configuracoes.ProxyUsuario = txtUsuario.Text;
                configuracoes.ProxySenha = txtSenha.Text;
                configuracoes.Fonte = cmbFont.SelectedItem.ToString();

                _repository.SalvarConfiguracoes(configuracoes);
            } catch (Exception ex) {
                HabilitarCampos();
                MostrarErro(ex.Message);
                return;
            }

            if (_frmDirtyTracker.PrecisaAtualizarFavoritos) {
                AtualizarFavoritos();
            }

            _formPrincipal.ConfigurarCoresForm();
            _formPrincipal.ConfigurarFonte();

            SplashCarregando.MudarMensagemStatus("Configurações salvas com sucesso.", 2000);
            SplashCarregando.FecharSplashCarregando();

            HabilitarCampos();

            Close();
        }

        private void AtualizarFavoritos() {
            var configuracoesAtuais = _repository.ObterConfiguracoes().First();

            _formPrincipal.MontarListaFavoritos();
        }

        private string SalvarCor(Color cor) {
            if (cor.IsNamedColor) {
                return cor.Name;
            }

            return string.Format("{0}, {1}, {2}", cor.R, cor.G, cor.B);
        }

        private string SalvarOpacidade() {
            double opacidade = 0;
            try {
                if (numOpacidade.Value == 100) {
                    double.TryParse(numOpacidade.Value.ToString(), out opacidade);
                } else {
                    double.TryParse("0," + (double)numOpacidade.Value, out opacidade);
                }
            } catch (Exception ex) {
                MostrarErro(ex.Message);
            }

            opacidade = Math.Abs(opacidade) < 0.1 ? 100 : opacidade;

            _formPrincipal.Opacity = opacidade;
            _formResultadosPesquisa.Opacity = opacidade;

            return opacidade.ToString();
        }

        private decimal CarregarOpacidade() {
            var opacidadeDouble = double.Parse(_repository.ObterConfiguracoes().First().Opacidade);
            decimal opacidade;
            if ((decimal)Math.Truncate(opacidadeDouble) == 100) {
                decimal.TryParse(opacidadeDouble.ToString(), out opacidade);
            } else {
                decimal.TryParse((opacidadeDouble * 100).ToString(), out opacidade);
            }

            return opacidade;
        }

        private bool ValidarCampos() {
            if (!ValidarConfiguracoesProxy()) { return false; }

            if (numOpacidade.Value < 10 || numOpacidade.Value > 100) {
                MostrarErro("O campo \"Opacidade da janela\" deve possuir um valor entre 10 e 100.");
                return false;
            }

            return true;
        }

        private bool ValidarConfiguracoesProxy() {
            if (cbProxy.Checked) {
                if (txtServidor.Text.IsNullEmptyOrWhiteSpace()) {
                    MostrarErro("Você deve preencher o Servidor das configurações de proxy.");
                    return false;
                }

                if (txtPorta.Text.IsNullEmptyOrWhiteSpace()) {
                    MostrarErro("Você deve preencher a Porta das configurações de proxy.");
                    return false;
                }
            }

            return true;
        }

        // Esta função foi criada somente para na hora de salvar os comandos na configuração do sistema
        // forçar a inclusão de um espaço ao final. Isto serve para comandos como o de calcular expressões matemáticas ou criar notas
        // pois após o espaço é que o sistema irá considerar a string de entrada. Caso o usuário tenha criado um comando com espaço no meio
        // do comando o sistema remove este espaço.
        private string ForcarEspacoEmComando(string comando, bool inserirEspacoFinal = false) {
            if (comando.IsNullEmptyOrWhiteSpace()) { return string.Empty; }

            var temMaisDeUmEspaco = comando.Split(' ').Count() > 1;
            if (temMaisDeUmEspaco)
                comando = comando.Replace(" ", "");

            var temEspacoNoFinal = comando.ToCharArray().Last().ToString() == " ";
            if (!temEspacoNoFinal && inserirEspacoFinal)
                comando = comando + " ";

            return comando;
        }

        private void DesabilitarCampos() {
            Enabled = false;
        }

        private void HabilitarCampos() {
            Enabled = true;
        }

        private bool SetStartup() {
            try {
                var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (key == null) { throw new Exception("A chave de registro não existe ou está bloqueada."); }

                if (cbIniciar.Checked) {
                    key.SetValue("Jaime.exe", Application.ExecutablePath);
                    return true;
                }

                key.DeleteValue("Jaime.exe", false);
                return false;
            } catch (Exception ex) {
                MostrarErro(ex.Message);
                return false;
            }
        }

        private void btnCorFundo_Click(object sender, EventArgs e) {
            var colorDialog = new ColorDialog();
            colorDialog.ShowHelp = false;
            colorDialog.Color = txtCorFundo.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                txtCorFundo.BackColor = colorDialog.Color;
            }
        }

        private void btnCorFonte_Click(object sender, EventArgs e) {
            var colorDialog = new ColorDialog();
            colorDialog.ShowHelp = false;
            colorDialog.Color = txtCorFonte.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                txtCorFonte.BackColor = colorDialog.Color;
            }
        }

        private void MostrarErro(string mensagem) {
            SplashCarregando.MudarMensagemStatus("", 100);
            SplashCarregando.FecharSplashCarregando();
            MessageBoxHelper.Error(mensagem);
            HabilitarCampos();
        }

        private void cbProxy_CheckedChanged(object sender, EventArgs e) {
            txtServidor.Enabled = cbProxy.Checked;
            txtPorta.Enabled = cbProxy.Checked;
            txtUsuario.Enabled = cbProxy.Checked;
            txtSenha.Enabled = cbProxy.Checked;
        }

        private void txtPorta_KeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void cmbFont_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                lblExemploFonte.Font = new Font(cmbFont.SelectedItem.ToString(), lblExemploFonte.Font.Size);
                lblExemploFonte.Text = "Exemplo referente a fonte escolhida";
            } catch (Exception) {
                lblExemploFonte.Text = "Não foi possível gerar a visualização";
                throw;
            }
        }

        private void btnRefazerBanco_Click(object sender, EventArgs e) {
            if (MessageBoxHelper.Alert("Esta operação irá limpar todos os registros de seu banco de dados. Deseja continuar?") == DialogResult.Yes) {
                Hide();

                DesabilitarCampos();

                var splashthread = new Thread(SplashCarregando.MostrarSplashCarregando) { IsBackground = true };
                splashthread.Start();

                SplashCarregando.MudarMensagemStatus("", 100);
                SplashCarregando.MudarMensagemStatus("Limpando banco de dados.", 2000);

                _repository.DeletarBanco();
                _repository.CriarBanco();

                _formPrincipal.MontarListaFavoritos();
                _formPrincipal.ConfigurarCoresForm();
                _formPrincipal.ConfigurarFonte();

                HabilitarCampos();
                SplashCarregando.FecharSplashCarregando();

                Close();
            }
        }

        private void Configuracoes_FormClosing(object sender, FormClosingEventArgs e) {
            if (_frmDirtyTracker.IsDirty && e.CloseReason == CloseReason.UserClosing && !_salvou) {
                switch (MessageBoxHelper.Question("Você deseja salvar as alterações antes de fechar?", MessageBoxButtons.YesNoCancel)) {
                    case DialogResult.Yes:
                        _salvou = true;
                        Salvar();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void tbBusca_Click(object sender, EventArgs e) {

        }

        private void btnAdicionarMotorBusca_Click(object sender, EventArgs e) {
            var motorBuscaSalvar = new MotorBuscaModel {
                Comando = ForcarEspacoEmComando(txtComandoBusca.Text, true),
                Parametros = txtParametros.Text,
                Url = txtUrl.Text
            };

            var comandosExistentes = _repository.ObterComandos().ToArray();
            if (comandosExistentes.Any(c => c.name.StartsWith(motorBuscaSalvar.Comando))) {
                MessageBoxHelper.Error("Este comando já está sendo utilizado.");
                return;
            }

            if (!motorBuscaSalvar.Url.Contains("{term}")) {
                MessageBoxHelper.Error("Você não inseriu o parâmetro \"{term}\" na url.");
                return;
            }

            if (motorBuscaSalvar.Url.IsNullEmptyOrWhiteSpace()) {
                MessageBoxHelper.Error("O campo \"Url\" é obrigatório.");
                return;
            }

            if (motorBuscaSalvar.Comando.IsNullEmptyOrWhiteSpace()) {
                MessageBoxHelper.Error("O campo \"Comando\" é obrigatório.");
                return;
            }

            txtUrl.Text = string.Empty;
            txtComandoBusca.Text = string.Empty;
            txtParametros.Text = string.Empty;
            txtUrl.Focus();

            _repository.SalvarMotorBusca(motorBuscaSalvar);

            _formPrincipal.AdicionarFavoritoMemoria(motorBuscaSalvar.Comando);

            CarregarMotoresBuscas();
        }

        private void btnExcluirMotorBusca_Click(object sender, EventArgs e) {
            btnExcluirMotorBusca.Enabled = false;

            if (MessageBoxHelper.Question("Tem certeza que deseja excluir os motores de busca selecionados?") == DialogResult.No) {
                btnExcluirMotorBusca.Enabled = true;
                return;
            }

            if (gridBuscas.RowCount <= 0) {
                MessageBoxHelper.Error("Não existem mais motores de busca para serem excluídos.");
                btnExcluirMotorBusca.Enabled = true;
            }

            var excluiu = false;
            foreach (DataGridViewRow row in gridBuscas.Rows) {
                var celulaSelecionar = (DataGridViewCheckBoxCell)row.Cells[0];
                var celulaId = (DataGridViewTextBoxCell)row.Cells[1];
                if ((bool)celulaSelecionar.Value) {
                    excluiu = true;
                    _repository.ExcluirMotorBusca(int.Parse(celulaId.Value.ToString()));
                }
            }

            if (!excluiu) {
                MessageBoxHelper.Error("Você deve selecionar algum motor de busca para excluir.");
            }

            btnExcluirMotorBusca.Enabled = true;
            CarregarMotoresBuscas();
        }

        private void btnRecarregarPlugins_Click(object sender, EventArgs e) {
            _formPrincipal.CarregarPlugins();
        }
    }
}
