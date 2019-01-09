using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using Dapper;
using Jaime.Enums;
using Jaime.Extensions;
using Jaime.Helpers;
using Jaime.Libs;
using Jaime.Models;
using Jaime.Properties;
using Jaime.Repository;
using Jaime.Service;
using JaimePlugin;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jaime {
    public partial class Principal : Form {
        #region Variaveis globais do form
        private System.Windows.Forms.Timer _timer;
        private int delay = 500;
        private readonly GlobalHotkey _globalHotkey;
        private KeyboardSimulator _keyboardSimulator;
        private bool _mostrar = true;
        private bool WinRStroked;
        public readonly HashSet<FavoritoChromeModel> FavoritosChrome = new HashSet<FavoritoChromeModel>();
        public readonly HashSet<FavoritoFirefoxModel> FavoritosFirefox = new HashSet<FavoritoFirefoxModel>();
        public readonly HashSet<MotorBuscaModel> MotoresBusca = new HashSet<MotorBuscaModel>();
        private readonly HashSet<string> _favoritosConfiguracoes = new HashSet<string>();
        private HashSet<string> _favoritosRegistro = new HashSet<string>();
        private readonly HashSet<ErrorModel> _erros = new HashSet<ErrorModel>();
        private string _primeiroFavoritoDaLista;
        private readonly ResultadosPesquisa _formResultados;
        private Carregando _formCarregando;
        private bool _atualizando;
        private readonly JaimeRepository _repository = new JaimeRepository();
        private readonly FavoritosHelper _favoritosExtension = new FavoritosHelper();
        private ConfiguracaoModel _configuracoes;
        public bool ModoDiretorio;
        public bool ModoArquivo;
        private bool _modoPlugin;
        private Image _iconePlugin;
        private HashSet<HistoricoFavoritoModel> HistoricoFavoritos = new HashSet<HistoricoFavoritoModel>();
        public List<IJaimePlugin> Plugins = new List<IJaimePlugin>();

        #region Variaveis para drag and drop do form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

        #region Variáveis para obter programas do sistema
        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        const int CSIDL_COMMON_PROGRAMS = 0x17;
        #endregion

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        #endregion

        public Principal() {
            Minimizar();

            _repository.CriarBanco();

            var splashthread = new Thread(SplashCarregando.MostrarSplashCarregando) { IsBackground = true };
            splashthread.Start();

            _configuracoes = _repository.ObterConfiguracoes().First();

            Opacity = Double.Parse(_configuracoes.Opacidade);

            InitializeComponent();
            _globalHotkey = new GlobalHotkey();
            MontarIconeSystemTray();
            _formResultados = new ResultadosPesquisa(this);

            Shown += (sender, args) => BringToFront();
        }

        private void Principal_Load(object sender, EventArgs e) {
            Inicializar();
            Minimizar();
        }

        public void Inicializar() {
            SplashCarregando.MudarMensagemStatus("Carregando o Jaime. Por favor, aguarde.");

            _globalHotkey.hookedKeyboardCallback += KListener_hookedKeyboardCallback;
            _keyboardSimulator = new KeyboardSimulator(new InputSimulator());

            CriarMenusDeContextoArquivos();
            CriarMenusDeContextoPastas();

            MontarListaFavoritos();
            ConfigurarCoresForm();
            ConfigurarFonte();
            CarregarPlugins();

            if (_erros.Any())
                SalvarLogDeErros();

            SplashCarregando.MudarMensagemStatus("Bem vindo ao Jaime!");
        }

        public void CarregarPlugins() {
            Plugins.Clear();

            var pluginType = typeof(IJaimePlugin);
            try {
                var caminho = Path.Combine(Application.StartupPath, "Plugins");
                if (!Directory.Exists(caminho)) {
                    Directory.CreateDirectory(caminho);
                }

                foreach (var plugin in Directory.EnumerateFiles(caminho, "*.dll")) {
                    SplashCarregando.MudarMensagemStatus("Carregando plugins.");

                    foreach (var type in Assembly.LoadFile(plugin).GetTypes()) {
                        if (type.GetInterface(pluginType.FullName) != null) {
                            var pluginCarregado = (IJaimePlugin)Activator.CreateInstance(type);
                            Plugins.Add(pluginCarregado);
                        }
                    }
                }
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("CarregarPlugins", $"Erro ao carregar plugins: {Environment.NewLine} Detalhes: {ex.Message}"));
            }
        }

        public void ConfigurarCoresForm() {
            SplashCarregando.MudarMensagemStatus("Configurando esquema de cores.");

            _configuracoes = _repository.ObterConfiguracoes().First();

            txtPesquisar.BackColor = _formResultados.BackColor = _formResultados.Controls["lbResultadosPesquisas"].BackColor = BackColor = JaimeHelper.ObterCor(_configuracoes.CorFundo);
            txtPesquisar.ForeColor = _formResultados.ForeColor = _formResultados.Controls["lbResultadosPesquisas"].ForeColor = ForeColor = JaimeHelper.ObterCor(_configuracoes.CorFonte);
        }

        public void ConfigurarFonte() {
            SplashCarregando.MudarMensagemStatus("Configurando fonte.");
            var fontName = _configuracoes.Fonte;
            var fontFamily = new InstalledFontCollection().Families.First();

            try {
                fontFamily = new FontFamily(fontName ?? "Arial");
            } catch (Exception) {
                fontFamily = new InstalledFontCollection().Families.First();
            }

            txtPesquisar.Font = new Font(fontFamily, 24);
            _formResultados.Controls["lbResultadosPesquisas"].Font = new Font(fontFamily, 10);

        }

        private bool KListener_hookedKeyboardCallback(KeyEvent keyevent, int vkcode, SpecialKeyState state) {
            if (keyevent == KeyEvent.WM_KEYDOWN || keyevent == KeyEvent.WM_SYSKEYDOWN) {
                if (state.AltPressed && vkcode == (int)Keys.Space) {
                    MostrarOcultarForm();
                    return false;
                }

                if (vkcode == (int)Keys.R && state.WinPressed) {
                    WinRStroked = true;

                    if (!_repository.ObterConfiguracoes().First().SubstituirTeclaWindowsR) {
                        return true;
                    }

                    if (!_mostrar)
                        MostrarOcultarForm();

                    txtPesquisar.Text = @"> ";
                    ReposicionarCursor();

                    return false;
                }
            }

            if (keyevent == KeyEvent.WM_KEYUP && WinRStroked && vkcode == (int)Keys.LWin) {
                WinRStroked = false;
                _keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.CONTROL);
                return false;
            }

            return true;
        }

        private void MostrarOcultarFormClick(object sender, EventArgs e) {
            MostrarOcultarForm();
        }

        private void MostrarOcultarForm() {
            TrocarIcone();

            if (!_mostrar) {
                Maximizar();
                Focus();
                WindowHelper.Activate(Handle);

                txtPesquisar.Focus();
            } else if (_mostrar) {
                Minimizar();
            }
        }

        private void CriarMenusDeContextoArquivos() {
            try {
                SplashCarregando.MudarMensagemStatus("Criando menu de contexto.");
                var diretorioRaiz = AppDomain.CurrentDomain.BaseDirectory;

                if (Registry.ClassesRoot.GetSubKeyNames().All(r => r != "*"))
                    Registry.ClassesRoot.CreateSubKey("*");

                var chaveTodos = Registry.ClassesRoot.OpenSubKey("*", true);

                if (chaveTodos.GetSubKeyNames().All(r => r != "shell"))
                    chaveTodos.CreateSubKey("shell");

                var chaveShell = chaveTodos.OpenSubKey("shell", true);

                #region Registrar
                if (chaveShell.GetSubKeyNames().All(r => r != "Adicionar ao Jaime"))
                    chaveShell.CreateSubKey("Adicionar ao Jaime");

                var chaveRegistrar = chaveShell.OpenSubKey("Adicionar ao Jaime", true);

                if (chaveRegistrar.GetValueNames().All(r => r != "Icon"))
                    chaveRegistrar.SetValue("Icon", diretorioRaiz + "Images\\icon-jaime.ico");

                if (chaveRegistrar.GetSubKeyNames().All(r => r != "command"))
                    chaveRegistrar.CreateSubKey("command");

                var chaveCommand = chaveRegistrar.OpenSubKey("command", true);
                chaveCommand.SetValue("", diretorioRaiz + "Jaime.exe %1");
                #endregion
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("CriarMenusDeContextoArquivos", ex.Message));
            }
        }

        private void CriarMenusDeContextoPastas() {
            try {
                var diretorioRaiz = AppDomain.CurrentDomain.BaseDirectory;

                if (Registry.ClassesRoot.GetSubKeyNames().All(r => r != "Directory"))
                    Registry.ClassesRoot.CreateSubKey("Directory");

                var chaveDiretorio = Registry.ClassesRoot.OpenSubKey("Directory", true);

                if (chaveDiretorio.GetSubKeyNames().All(r => r != "shell"))
                    chaveDiretorio.CreateSubKey("shell");

                var chaveShell = chaveDiretorio.OpenSubKey("shell", true);

                #region Registrar
                if (chaveShell.GetSubKeyNames().All(r => r != "Adicionar ao Jaime"))
                    chaveShell.CreateSubKey("Adicionar ao Jaime");

                var chaveRegistrar = chaveShell.OpenSubKey("Adicionar ao Jaime", true);

                if (chaveRegistrar.GetValueNames().All(r => r != "Icon"))
                    chaveRegistrar.SetValue("Icon", diretorioRaiz + "Images\\icon-jaime.ico");

                if (chaveRegistrar.GetSubKeyNames().All(r => r != "command"))
                    chaveRegistrar.CreateSubKey("command");

                var commandRegistrar = chaveRegistrar.OpenSubKey("command", true);
                commandRegistrar.SetValue("", diretorioRaiz + "Jaime.exe %1");
                #endregion
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("CriarMenusDeContextoPastas", ex.Message));
            }
        }

        public void MontarListaFavoritos() {
            try {
                _atualizando = true;
                BloquearDesbloquearFuncoes(true);
                SplashCarregando.MudarMensagemStatus("Montando lista de favoritos.");
                _favoritosRegistro.Clear();
                var lb = _formResultados.Controls["lbResultadosPesquisas"] as ListBox;
                lb.DataSource = null;

                var favoritos = new HashSet<string>();
                var dataSource = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\JaimeFavoritos\\JaimeFavoritos.sqlite";

                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))) {
                    if (File.Exists(dataSource)) {
                        var jaimeFavoritos = _repository.ObterFavoritos();
                        if (jaimeFavoritos.Any()) {
                            favoritos.UnionWith(jaimeFavoritos.Select(f => f.Nome).ToArray());
                        }
                    }
                }

                ObterMotoresBusca();
                if (MotoresBusca != null && MotoresBusca.Any())
                    favoritos.UnionWith(MotoresBusca.Select(ff => ff.Comando).ToArray());

                ObterFavoritosFirefox();
                if (FavoritosFirefox != null && FavoritosFirefox.Any())
                    favoritos.UnionWith(FavoritosFirefox.Select(ff => ff.title).ToArray());

                ObterFavoritosChrome();
                if (FavoritosChrome != null && FavoritosChrome.Any())
                    favoritos.UnionWith(FavoritosChrome.Select(fn => fn.Name).ToArray());

                ComandosConfigurados();
                if (_favoritosConfiguracoes != null && _favoritosConfiguracoes.Any())
                    favoritos.UnionWith(_favoritosConfiguracoes.ToArray());

                var favoritosSistema = ObterFavoritosSistema();
                if (favoritosSistema != null && favoritosSistema.Any()) {
                    SplashCarregando.MudarMensagemStatus("Atualizando lista de favoritos do sistema.");
                    _repository.AtualizarFavoritosSistema(favoritosSistema.ToArray());
                }

                _favoritosRegistro.UnionWith(favoritos);
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("MontarListaFavoritos", ex.Message));
            }

            BloquearDesbloquearFuncoes(false);
            _atualizando = false;
        }

        private List<FavoritoModel> ObterFavoritosSistema() {
            var favoritosRetorno = new List<FavoritoModel>();

            try {
                var programs = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "*.*", SearchOption.AllDirectories);

                favoritosRetorno.AddRange(programs.Select(p => new FavoritoModel {
                    Nome = Path.GetFileNameWithoutExtension(p.Split('\\').Last()),
                    Caminho = p,
                    QuantidadeVezesAberto = 0,
                    Tipo = TipoFavorito.Sistema
                }));
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("MontarListaFavoritos", ex.Message));
            }

            try {
                var commonStartMenuPath = new StringBuilder(560);
                SHGetSpecialFolderPath(IntPtr.Zero, commonStartMenuPath, CSIDL_COMMON_PROGRAMS, false);
                var path = commonStartMenuPath.ToString();
                var programs = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);

                favoritosRetorno.AddRange(programs.Select(p => new FavoritoModel {
                    Nome = Path.GetFileNameWithoutExtension(p.Split('\\').Last()),
                    Caminho = p,
                    QuantidadeVezesAberto = 0,
                    Tipo = TipoFavorito.Sistema
                }).ToList());
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("MontarListaFavoritos", ex.Message));
            }

            try {
                ReadAppPaths(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths", favoritosRetorno);
                ReadAppPaths(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\App Paths", favoritosRetorno);
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("MontarListaFavoritos", ex.Message));
            }

            return favoritosRetorno;
        }

        private void ReadAppPaths(string rootpath, List<FavoritoModel> list) {
            using (var root = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(rootpath)) {
                if (root == null) return;
                foreach (var item in root.GetSubKeyNames()) {
                    using (var key = root.OpenSubKey(item)) {
                        object path = key.GetValue("");
                        if (path is string && global::System.IO.File.Exists((string)path)) {
                            var fileInfo = new FileInfo(path.ToString());
                            list.Add(new FavoritoModel {
                                Caminho = Path.GetFileNameWithoutExtension(path.ToString()),
                                Nome = fileInfo.Name,
                                QuantidadeVezesAberto = 0,
                                Tipo = TipoFavorito.Sistema
                            });
                        }

                        key.Close();
                    }
                }
            }
        }

        private FileInfo TryGetFileInfo(string path) {
            if (string.IsNullOrEmpty(path)) { return null; }

            try {
                var info = new FileInfo(path);
                return info;
            } catch (Exception ex) {
                return null;
            }
        }

        private void BloquearDesbloquearFuncoes(bool bloquear) {
            if (_formResultados.InvokeRequired) {
                _formResultados.BeginInvoke((MethodInvoker)(() => _formResultados.Hide()));
            } else {
                _formResultados.Hide();
            }

            if (txtPesquisar.InvokeRequired) {
                txtPesquisar.BeginInvoke((MethodInvoker)delegate {
                    txtPesquisar.Text = !bloquear ? string.Empty : "Atualizando lista. Por favor, aguarde.";
                    txtPesquisar.Enabled = !bloquear;
                });
            } else {
                txtPesquisar.Text = !bloquear ? string.Empty : "Atualizando lista. Por favor, aguarde.";
                txtPesquisar.Enabled = !bloquear;
            }
        }

        private void ComandosConfigurados() {
            _favoritosConfiguracoes.Clear();
            _configuracoes = _repository.ObterConfiguracoes().First();

            SplashCarregando.MudarMensagemStatus("Montando lista de comandos configurados.");
            _favoritosConfiguracoes.UnionWith(_repository.ObterComandos().Select(c => c.name).ToArray());

            //Adiciona comando padrão para abrir configurações
            _favoritosConfiguracoes.Add("config");
            _favoritosConfiguracoes.Add("close");
        }

        private void ObterFavoritosChrome() {
            FavoritosChrome.Clear();

            try {
                if (_configuracoes.FavoritosNavegadores) {
                    SplashCarregando.MudarMensagemStatus("Montando lista de favoritos do Chrome.");
                    var google = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\";

                    if (!Directory.Exists(google)) { throw new Exception("O diretório do Google Chrome não foi encontrado"); }

                    var googleBookmarksFile = Directory.GetFiles(google, "*.*", SearchOption.AllDirectories).FirstOrDefault(f => f.Contains("Bookmarks"));

                    if (googleBookmarksFile.IsNullEmptyOrWhiteSpace()) { throw new Exception("O arquivo de favoritos do Google Chrome não foi encontrado"); }

                    var reader = new StreamReader(googleBookmarksFile).ReadToEnd();
                    var json = JsonConvert.DeserializeObject(reader);
                    var jsonObject = JObject.Parse(json.ToString());
                    var firstChilds = jsonObject.SelectToken("roots").SelectToken("bookmark_bar").SelectMany(c => c).ToArray();

                    MontarFavoritosChrome(firstChilds);
                }
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("FatoritosChrome", ex.Message));
            }
        }

        private void ObterFavoritosFirefox() {
            FavoritosFirefox.Clear();

            if (_configuracoes.FavoritosNavegadores) {
                SplashCarregando.MudarMensagemStatus("Montando lista de favoritos do Firefox.");

                var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Roaming\Mozilla\Firefox\Profiles\";

                if (!Directory.Exists(path)) {
                    return;
                }

                var dbs = Directory.EnumerateFiles(path, "places.sqlite", SearchOption.AllDirectories);

                if (!dbs.Any()) {
                    return;
                }

                foreach (var db in dbs) {
                    try {
                        using (var cnn = new SQLiteConnection("DataSource=" + db)) {
                            cnn.Open();
                            var places = cnn.Query<FavoritoFirefoxModel>(@"
                                SELECT * FROM moz_places
                            ");

                            FavoritosFirefox.UnionWith(places.Where(p => !string.IsNullOrEmpty(p.title)).ToArray());
                        }
                    } catch (Exception ex) {
                        _erros.Add(new ErrorModel("FatoritosFirefox", ex.Message));
                    }
                }
            }
        }

        private void ObterMotoresBusca() {
            MotoresBusca.Clear();

            var motoresBusca = _repository.ObterMotoresBuscas().ToArray();

            if (!motoresBusca.Any()) return;

            foreach (var motor in motoresBusca) {
                MotoresBusca.Add(motor);
            }
        }

        private void MontarFavoritosChrome(IEnumerable<JToken> childs) {
            foreach (var child in childs) {
                var nextChilds = child.SelectTokens("children").ToArray();
                if (nextChilds.Any()) {
                    MontarFavoritosChrome(nextChilds);
                } else {
                    var values = child.ToArray();
                    foreach (var value in values) {
                        nextChilds = value.SelectTokens("children").ToArray();
                        if (nextChilds.Any()) {
                            MontarFavoritosChrome(nextChilds);
                        } else if (value.SelectToken("type") != null && value.SelectToken("type").ToString() != "{folder}") {
                            var url = value.SelectToken("url") != null ? value.SelectToken("url").ToString() : string.Empty;
                            var name = value.SelectToken("name") != null ? value.SelectToken("name").ToString() : string.Empty;
                            var id = value.SelectToken("id") != null ? value.SelectToken("id").ToString() : string.Empty;
                            if (!url.IsNullEmptyOrWhiteSpace() && !name.IsNullEmptyOrWhiteSpace() && !id.IsNullEmptyOrWhiteSpace()) {
                                FavoritosChrome.Add(new FavoritoChromeModel {
                                    Url = url.Replace("http://", "").Replace("https://", "").Replace("www.", ""),
                                    Name = name,
                                    Id = id
                                });
                            }
                        }
                    }
                }
            }
        }

        public bool VerificarUrl(string url, ref string protocoloRetorno) {
            var protocolos = new[] { "http://", "https://", "http://www.", "https://www." };

            foreach (var protocolo in protocolos) {
                if (url.IndexOf(protocolo) != -1) { url = url.Replace(protocolo, ""); }

                var wr = (HttpWebRequest)WebRequest.Create(protocolo + url);
                wr.Timeout = 10000;
                wr.Credentials = CredentialCache.DefaultNetworkCredentials;

                try {
                    var resposta = wr.GetResponse();
                    protocoloRetorno = protocolo;
                    resposta.Close();
                    return true;
                } catch (Exception ex) {
                    _erros.Add(new ErrorModel("VerificarUrl", ex.Message, protocolo));
                }
            }

            return false;
        }

        private void Principal_VisibleChanged(object sender, EventArgs e) {
            try {
                if (Visible == false) {
                    _formResultados.Hide();
                }

                if (!_atualizando) {
                    txtPesquisar.Text = "";
                    txtPesquisar.Focus();
                }
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("VerificarUrl", ex.Message));
            }
        }

        private void sysIcon_MouseDoubleClick(object sender, MouseEventArgs e) {
            MostrarOcultarFormClick(sender, e);
        }

        private void Fechar(object Sender, EventArgs e) {
            FecharJaime();
        }

        public void Minimizar() {
            _mostrar = false;
            Hide();
        }

        public void Maximizar() {
            _mostrar = true;
            Show();
        }

        private void AtualizarFavoritos(object Sender, EventArgs e) {
            Minimizar();

            var splashthread = new Thread(SplashCarregando.MostrarSplashCarregando) { IsBackground = true };
            splashthread.Start();

            SplashCarregando.MudarMensagemStatus("");
            SplashCarregando.MudarMensagemStatus("Atualizando lista de favoritos.");

            MontarListaFavoritos();

            SplashCarregando.MudarMensagemStatus("Lista atualizada com sucesso.");
            SplashCarregando.FecharSplashCarregando();
        }

        public void FecharJaime() {
            sysIcone.Icon = null;
            Close();
        }

        private void VisualizarFavoritos(object sender, EventArgs e) {
            try {
                _mostrar = false;
                Minimizar();
                var jaimeFavoritos = _repository.ObterFavoritos().Where(f => f.Tipo != TipoFavorito.Sistema);
                if (jaimeFavoritos.Any()) {
                    var formFavoritos = new Favoritos(this);
                    var grid = (DataGridView)formFavoritos.Controls["gridFavoritos"];

                    foreach (var favorito in jaimeFavoritos) {
                        if (favorito.Nome == null) {
                            continue;
                        }

                        grid.Rows.Add(false, favorito.Nome, favorito.Caminho);
                    }

                    grid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    grid.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    formFavoritos.Show();
                } else {
                    MessageBoxHelper.Info("Nenhum favorito foi adicionado ainda.");
                }
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("VisualizarFavoritos", ex.Message));
            }
        }

        private void btnConfiguracoes_Click(object sender, EventArgs e) {
            AbrirJanelaConfiguracoes();
        }

        public void AbrirJanelaConfiguracoes() {
            var frmConfiguracoes = new Configuracoes(this, _formResultados);
            _formResultados.Hide();
            _mostrar = false;
            Minimizar();
            frmConfiguracoes.Show();
        }

        private void MontarIconeSystemTray() {
            try {
                var menu = new ContextMenu();

                var menuAbrir = new MenuItem();
                var menuVisualizarFavoritos = new MenuItem();
                var menuFechar = new MenuItem();
                var menuConfiguracoes = new MenuItem();
                var menuAtualizar = new MenuItem();

                var listaMenus = new[] { menuAbrir, menuVisualizarFavoritos, menuConfiguracoes, menuAtualizar, menuFechar };
                menu.MenuItems.AddRange(listaMenus);

                menuAbrir.Index = 0;
                menuAbrir.Text = "&Mostrar/Ocultar";
                menuAbrir.Click += MostrarOcultarFormClick;

                menuVisualizarFavoritos.Index = 1;
                menuVisualizarFavoritos.Text = "&Visualizar Favoritos";
                menuVisualizarFavoritos.Click += VisualizarFavoritos;

                menuConfiguracoes.Index = 2;
                menuConfiguracoes.Text = "&Configurações";
                menuConfiguracoes.Click += btnConfiguracoes_Click;

                menuAtualizar.Index = 3;
                menuAtualizar.Text = "A&tualizar favoritos";
                menuAtualizar.Click += AtualizarFavoritos;

                menuFechar.Index = 4;
                menuFechar.Text = "&Fechar";
                menuFechar.Click += Fechar;

                sysIcone.ContextMenu = menu;
            } catch (Exception ex) {
                _erros.Add(new ErrorModel("MontarIconeSystemTray", ex.Message));
            }
        }

        private void Principal_FormClosing(object sender, FormClosingEventArgs e) {
            sysIcone.Icon = null;
            ExcluirMenuContextoArquivo();
            ExcluirMenuContextoDiretorio();
        }

        private void txtPesquisar_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Enter) {
                // Necessário para parar o Ding do ENTER
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            //Caso tecle ESC no txtPesquisar, o sistema entende que é para esconder o form
            if (e.KeyCode == Keys.Escape) {
                _primeiroFavoritoDaLista = string.Empty;
                Minimizar();
                Activate();
                _mostrar = false;
            }

            var lista = _formResultados.Controls["lbResultadosPesquisas"] as ListBox;
            var texto = txtPesquisar.Text;
            if (texto.IsNullEmptyOrWhiteSpace()) {
                _formResultados.Hide();
                TrocarIcone();
            }

            if (e.KeyCode == Keys.Down) {
                if (lista.Items.Count <= 0) { return; }

                _formResultados.Focus();
                lista.Focus();
                var index = lista.Items.Count > 1 ? 1 : 0;

                if (ModoArquivo || ModoDiretorio)
                    index = 0;

                lista.SelectedIndex = index;

                return;
            }

            if (e.KeyCode == Keys.Up) {
                SelecionarHistoricoFavorito();
                return;
            }

            if (texto.IsNullEmptyOrWhiteSpace()) {
                _primeiroFavoritoDaLista = string.Empty;
                return;
            }
        }

        public void AtualizarPosicaoLista() {
            var x = txtPesquisar.PointToScreen(Point.Empty).X;
            var y = txtPesquisar.PointToScreen(Point.Empty).Y + txtPesquisar.Height + 4;
            var width = txtPesquisar.Width;

            if (_formResultados != null) {
                _formResultados.Location = new Point(x, y);
                _formResultados.Width = width;
            }
        }

        private IEnumerable<FavoritoModel> PesquisarPorIniciais(string texto, FavoritoModel[] favoritos, int posicao) {
            texto = texto.Trim();

            var retorno = new List<FavoritoModel>();
            var contagem = texto.Length;
            var tamanhoOriginal = texto.Length;
            var sobrenome = string.Empty;
            var encontrou = false;

            while (!encontrou && contagem > 0) {
                foreach (var favorito in favoritos) {
                    var palavras = favorito.Nome.Trim().Split(' ').Where(f => !string.IsNullOrEmpty(f)).ToArray();

                    if (palavras.Count() < posicao)
                        continue;

                    var stringPesquisar = texto.Substring(0, contagem).ToLower();

                    if (stringPesquisar.Length != tamanhoOriginal && !string.IsNullOrEmpty(stringPesquisar))
                        sobrenome = texto.Replace(stringPesquisar, "");

                    if (palavras.Count() > posicao && palavras[posicao].ToLower().Contains(stringPesquisar))
                        retorno.Add(favorito);
                }

                encontrou = retorno.Any();
                contagem -= 1;
            }

            if (!encontrou || sobrenome.IsNullEmptyOrWhiteSpace()) return retorno.Distinct().ToArray();

            var retornosFiltrados = PesquisarPorIniciais(sobrenome, retorno.ToArray(), posicao + 1);
            retorno.Clear();
            retorno.AddRange(retornosFiltrados);

            return retorno.Distinct().ToArray();
        }

        private HashSet<string> FiltrarFavoritos(string texto) {
            _primeiroFavoritoDaLista = string.Empty;

            var favoritosRetorno = new HashSet<string>();
            var functions = new FavoritoService(this);

            //Caso seja comandos que são expecíficos do sistema, nem retorna nenhum favorito
            if (functions.VerificarComandoConfigurado(texto)) { return favoritosRetorno; }

            if ((ModoDiretorio || ModoArquivo || Directory.Exists(texto) || File.Exists(texto)) && texto.Contains("\\")) {
                favoritosRetorno.UnionWith(PegarListaDiretoriosEArquivos(texto));
                _primeiroFavoritoDaLista = favoritosRetorno.FirstOrDefault();
                return favoritosRetorno;
            }

            ModoDiretorio = false;
            ModoArquivo = false;

            var favoritosDiretorios = _repository.ObterFavoritosEspecificos(texto).OrderByDescending(f => f.QuantidadeVezesAberto);
            var favoritosFiltrar = _favoritosRegistro.Concat(favoritosDiretorios.Select(f => f.Nome).ToArray());

            var favoritoExato = favoritosFiltrar.Where(f => f.ToLower().StartsWith(texto.ToLower(), StringComparison.CurrentCultureIgnoreCase) && f.Length == texto.Length).ToList();
            var favoritosComecados = favoritosFiltrar.Where(f => f.ToLower().StartsWith(texto.ToLower(), StringComparison.CurrentCultureIgnoreCase)).ToList();
            var favoritos = favoritosFiltrar.Where(f => f.ToLower().Contains(texto.ToLower())).ToList();

            if (favoritosDiretorios.Any())
                favoritosRetorno.UnionWith(favoritosDiretorios.Select(fd => fd.Nome).ToArray());

            if (favoritoExato.Any())
                favoritosRetorno.UnionWith(favoritoExato);

            if (favoritosComecados.Any())
                favoritosRetorno.UnionWith(favoritosComecados);

            if (favoritos.Any())
                favoritosRetorno.UnionWith(favoritos);

            // Caso não encontre nenhum favorito, o sistema utiliza um algoritmo mais inteligente para pesquisar por abreviações
            // Pareciso com o resharper
            if (!favoritosRetorno.Any()) {
                var favoritosIniciais = PesquisarPorIniciais(texto, _repository.ObterFavoritos().ToArray(), 0);
                favoritosRetorno.UnionWith(favoritosIniciais.OrderByDescending(f => f.QuantidadeVezesAberto).Select(f => f.Nome).ToArray());
            }

            _primeiroFavoritoDaLista = favoritosRetorno.FirstOrDefault();

            return favoritosRetorno;
        }

        private HashSet<string> PegarListaDiretoriosEArquivos(string path) {
            var diretoriosArquivos = new HashSet<string>();
            var texto = txtPesquisar.Text.Split('\\');
            var lastIndex = texto.Count();
            var pattern = "*.*";
            var filter = string.Empty;

            if (lastIndex > 1) {
                if (!texto[lastIndex - 1].IsNullEmptyOrWhiteSpace()) {
                    filter = texto[lastIndex - 1];
                    path = path.Replace(texto[lastIndex - 1], "");
                }
            }

            if (Directory.Exists(path)) {
                ModoDiretorio = true;
                ModoArquivo = false;

                if (DiretorioTemPermissaoLeitura(path)) {
                    var diretorios = Directory.EnumerateDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

                    if (!filter.IsNullEmptyOrWhiteSpace())
                        diretorios = diretorios.Where(d => d.ToLower().StartsWith(path.ToLower() + filter.ToLower()));

                    foreach (var dir in diretorios) {
                        if (DiretorioTemPermissaoLeitura(dir))
                            diretoriosArquivos.Add(dir);
                    }

                    var arquivos = Directory.EnumerateFiles(path, pattern, SearchOption.TopDirectoryOnly);

                    if (!filter.IsNullEmptyOrWhiteSpace())
                        arquivos = arquivos.Where(d => d.ToLower().StartsWith(path.ToLower() + filter.ToLower()));

                    foreach (var file in arquivos) {
                        if (ArquivoTemPermissaoLeitura(file))
                            diretoriosArquivos.Add(file);
                    }
                }

                return diretoriosArquivos;
            }

            ModoDiretorio = false;
            ModoArquivo = true;

            diretoriosArquivos.Add(path);

            return diretoriosArquivos;
        }

        public static bool ArquivoTemPermissaoLeitura(string path) {
            try {
                var writeAllow = false;
                var writeDeny = false;
                var accessControlList = File.GetAccessControl(path);
                if (accessControlList == null)
                    return false;

                var accessRules = accessControlList.GetAccessRules(true, true, typeof(SecurityIdentifier));

                foreach (FileSystemAccessRule rule in accessRules) {
                    if ((FileSystemRights.ReadAndExecute & rule.FileSystemRights) != FileSystemRights.ReadAndExecute)
                        continue;

                    if (rule.AccessControlType == AccessControlType.Allow)
                        writeAllow = true;
                    else if (rule.AccessControlType == AccessControlType.Deny)
                        writeDeny = true;
                }

                return writeAllow && !writeDeny;
            } catch (Exception) {
                return false;
            }
        }

        public static bool DiretorioTemPermissaoLeitura(string path) {
            try {
                var writeAllow = false;
                var writeDeny = false;
                var accessControlList = Directory.GetAccessControl(path);
                if (accessControlList == null)
                    return false;

                var accessRules = accessControlList.GetAccessRules(true, true, typeof(SecurityIdentifier));

                if (!Directory.EnumerateDirectories(path).Any() && !Directory.EnumerateFiles(path).Any()) { return false; }

                foreach (FileSystemAccessRule rule in accessRules) {
                    if ((FileSystemRights.ReadAndExecute & rule.FileSystemRights) != FileSystemRights.ReadAndExecute)
                        continue;

                    if (rule.AccessControlType == AccessControlType.Allow)
                        writeAllow = true;
                    else if (rule.AccessControlType == AccessControlType.Deny)
                        writeDeny = true;
                }

                return writeAllow && !writeDeny;
            } catch (Exception) {
                return false;
            }
        }

        public void TrocarIcone() {
            var partesFavorito = txtPesquisar.Text.Split(' ');
            var favorito = string.Empty;

            if (partesFavorito.Count() > 1)
                favorito = partesFavorito.First() + " ";
            else {
                favorito = txtPesquisar.Text;
            }

            if (favorito.IsNullEmptyOrWhiteSpace()) {
                pbIcone.Image = Resources.Jaime;
                return;
            }

            try {
                if (favorito.ToLower() == "close") {
                    pbIcone.Image = Resources.Fechar;
                    return;
                }

                if (favorito.ToLower() == "config") {
                    pbIcone.Image = Resources.Configuration;
                    return;
                }

                if (favorito.ToLower().StartsWith("> ")) {
                    pbIcone.Image = Resources.PromptComando;
                    return;
                }

                var favoritos = _repository.ObterFavoritosEspecificos(favorito);
                var caminho = favoritos.Where(f => f.Nome.ToLower() == favorito.ToLower()).Select(f => f.Caminho).FirstOrDefault();

                if (!caminho.IsNullEmptyOrWhiteSpace() && Directory.Exists(caminho)) {
                    pbIcone.Image = Resources.Folder;
                    return;
                }

                if (!caminho.IsNullEmptyOrWhiteSpace() && File.Exists(caminho)) {
                    pbIcone.Image = Icon.ExtractAssociatedIcon(caminho).ToBitmap();
                    return;
                }

                if (Directory.Exists(favorito)) {
                    pbIcone.Image = Resources.Folder;
                    return;
                }

                if (File.Exists(favorito)) {
                    var extractAssociatedIcon = Icon.ExtractAssociatedIcon(favorito);
                    if (extractAssociatedIcon != null) {
                        pbIcone.Image = extractAssociatedIcon.ToBitmap();
                        return;
                    }
                }

                var favoritoDiretorio = _repository.ObterFavoritos().Where(f => f.Nome == favorito).Select(f => new { f.Caminho, f.Nome }).FirstOrDefault();
                if (favoritoDiretorio != null) {
                    if (Directory.Exists(favoritoDiretorio.Caminho)) {
                        pbIcone.Image = Resources.Folder;
                        return;
                    }

                    var extractAssociatedIcon = Icon.ExtractAssociatedIcon(favoritoDiretorio.Caminho);
                    if (extractAssociatedIcon != null) {
                        pbIcone.Image = extractAssociatedIcon.ToBitmap();
                        return;
                    }
                }

                var motorBusca = _repository.ObterMotoresBuscas().FirstOrDefault(c => c.Comando.StartsWith(favorito))?.Comando ?? string.Empty;
                if (!string.IsNullOrEmpty(motorBusca)) {
                    pbIcone.Image = Resources.Search;
                    return;
                }

                if (FavoritosChrome.Any(fn => fn.Name.StartsWith(favorito)) || FavoritosFirefox.Any(fn => fn.title.StartsWith(favorito))) {
                    var icone = PegarNavegadorPadrao();
                    pbIcone.Image = icone?.ToBitmap() ?? Resources.Jaime;
                    return;
                }

                if (_modoPlugin && _iconePlugin != null) {
                    pbIcone.Image = _iconePlugin ?? Resources.Jaime;
                    return;
                }

                var plugin = Plugins.FirstOrDefault(p => p.Comando.StartsWith(favorito));
                if (plugin != null) {
                    pbIcone.Image = plugin.Icone ?? Resources.Jaime;
                    return;
                }

                pbIcone.Image = Resources.Jaime;
            } catch (Exception ex) {
                pbIcone.Image = Resources.Jaime;
            }
        }

        public void TrocarIconeAoMoverLista(string favorito = "") {
            if (favorito.IsNullEmptyOrWhiteSpace()) {
                pbIcone.Image = Resources.Jaime;
                return;
            }

            try {
                if (favorito.ToLower() == "close") {
                    pbIcone.Image = Resources.Fechar;
                    return;
                }

                if (favorito.ToLower() == "config") {
                    pbIcone.Image = Resources.Configuration;
                    return;
                }

                if (favorito.ToLower().StartsWith("> ")) {
                    pbIcone.Image = Resources.PromptComando;
                    return;
                }

                var favoritos = _repository.ObterFavoritosEspecificos(favorito);
                var caminho = favoritos.Where(f => f.Nome.ToLower() == favorito.ToLower()).Select(f => f.Caminho).FirstOrDefault();

                if (!caminho.IsNullEmptyOrWhiteSpace() && Directory.Exists(caminho)) {
                    pbIcone.Image = Resources.Folder;
                    return;
                }

                if (!caminho.IsNullEmptyOrWhiteSpace() && File.Exists(caminho)) {
                    pbIcone.Image = Icon.ExtractAssociatedIcon(caminho).ToBitmap();
                    return;
                }

                if (Directory.Exists(favorito)) {
                    pbIcone.Image = Resources.Folder;
                    return;
                }

                if (File.Exists(favorito)) {
                    var extractAssociatedIcon = Icon.ExtractAssociatedIcon(favorito);
                    if (extractAssociatedIcon != null) {
                        pbIcone.Image = extractAssociatedIcon.ToBitmap();
                        return;
                    }
                }

                var favoritoDiretorio = _repository.ObterFavoritos().Where(f => f.Nome == favorito).Select(f => new { f.Caminho, f.Nome }).FirstOrDefault();
                if (favoritoDiretorio != null) {
                    if (Directory.Exists(favoritoDiretorio.Caminho)) {
                        pbIcone.Image = Resources.Folder;
                        return;
                    }

                    var extractAssociatedIcon = Icon.ExtractAssociatedIcon(favoritoDiretorio.Caminho);
                    if (extractAssociatedIcon != null) {
                        pbIcone.Image = extractAssociatedIcon.ToBitmap();
                        return;
                    }
                }

                var motorBusca = _repository.ObterMotoresBuscas().FirstOrDefault(c => c.Comando.StartsWith(favorito))?.Comando ?? string.Empty;
                if (!string.IsNullOrEmpty(motorBusca)) {
                    pbIcone.Image = Resources.Search;
                    return;
                }

                if (FavoritosChrome.Any(fn => fn.Name.StartsWith(favorito)) || FavoritosFirefox.Any(fn => fn.title.StartsWith(favorito))) {
                    var icone = PegarNavegadorPadrao();
                    pbIcone.Image = icone?.ToBitmap() ?? Resources.Jaime;
                    return;
                }

                if (_modoPlugin && _iconePlugin != null) {
                    pbIcone.Image = _iconePlugin ?? Resources.Jaime;
                    return;
                }

                var plugin = Plugins.FirstOrDefault(p => p.Comando.StartsWith(favorito));
                if (plugin != null) {
                    pbIcone.Image = plugin.Icone ?? Resources.Jaime;
                    return;
                }

                pbIcone.Image = Resources.Jaime;
            } catch (Exception ex) {
                pbIcone.Image = Resources.Jaime;
            }
        }

        private Icon PegarNavegadorPadrao() {
            const string userChoice = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice";
            string progId;
            using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(userChoice, true)) {
                if (userChoiceKey == null) {
                    return null;
                }

                object progIdValue = userChoiceKey.GetValue("Progid");

                if (progIdValue == null) {
                    return null;
                }

                progId = progIdValue.ToString();
            }

            return PegarIconeBrowserPadrao(progId);
        }

        private Icon PegarIconeBrowserPadrao(string progId) {
            const string exeSuffix = ".exe";
            string caminhoBrowser = progId + @"\shell\open\command";
            FileInfo browserPath;
            using (RegistryKey pathKey = Registry.ClassesRoot.OpenSubKey(caminhoBrowser, true)) {
                if (pathKey == null) {
                    return null;
                }

                try {
                    string path = pathKey.GetValue(null).ToString().ToLower().Replace("\"", "");
                    if (!path.EndsWith(exeSuffix)) {
                        path = path.Substring(0, path.LastIndexOf(exeSuffix, StringComparison.Ordinal) + exeSuffix.Length);
                        browserPath = TryGetFileInfo(path);
                        return Icon.ExtractAssociatedIcon(browserPath.ToString());
                    }
                } catch {
                    return null;
                }
            }

            return null;
        }

        private void SalvarLogDeErros() {
            SplashCarregando.MudarMensagemStatus("Montando log de erros.");
            const string diretorio = @"C:\ErrosJaime";
            if (!Directory.Exists(diretorio)) {
                Directory.CreateDirectory(diretorio);
            }

            const string nomeArquivo = diretorio + @"\ErrosJaime.txt";
            var arquivo = new FileStream(nomeArquivo, FileMode.Create, FileAccess.ReadWrite);
            using (var sw = new StreamWriter(arquivo)) {
                try {
                    foreach (var error in _erros) {
                        var linha = string.Format("{0} | {1} | {2} | {3}", DateTime.Now, error.Local, error.Mensagem, error.Detalhes);
                        sw.WriteLine(linha);
                    }
                } catch (Exception ex) {
                    MessageBoxHelper.Error(ex.Message);
                }
            }

            arquivo.Close();

            if (MessageBoxHelper.Error("Ocorreram erros durante o processo, deseja visualizar o arquivo de log?", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                Process.Start(nomeArquivo);
            }
        }

        private void Principal_Shown(object sender, EventArgs e) {
            SplashCarregando.FecharSplashCarregando();
            sysIcone.Visible = true;
        }

        // Ao mover o form, atualiza a posição da lista de resultados
        // Foi necessário para que a lista acompanhe o form quando aberta
        private void Principal_Move(object sender, EventArgs e) {
            AtualizarPosicaoLista();
        }

        //Função criada para poder arrastar o form principal clicando e segurando no icone
        private void pbIcone_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public void ReposicionarCursor(bool limpar = false) {
            txtPesquisar.Focus();

            if (limpar && !_atualizando) {
                txtPesquisar.Text = string.Empty;
                return;
            }

            txtPesquisar.SelectionStart = txtPesquisar.Text.Length;
            txtPesquisar.SelectionLength = 0;
        }

        private void ExcluirMenuContextoArquivo() {
            if (Registry.ClassesRoot.GetSubKeyNames().All(r => r != "*")) { return; }

            var chaveTodos = Registry.ClassesRoot.OpenSubKey("*", true);

            if (chaveTodos.GetSubKeyNames().All(r => r != "shell")) { return; }

            var chaveShell = chaveTodos.OpenSubKey("shell", true);

            if (chaveShell.GetSubKeyNames().Any(r => r == "Adicionar ao Jaime"))
                chaveShell.DeleteSubKeyTree("Adicionar ao Jaime");
        }

        private void ExcluirMenuContextoDiretorio() {
            if (Registry.ClassesRoot.GetSubKeyNames().All(r => r != "*")) { return; }

            var chageDirectory = Registry.ClassesRoot.OpenSubKey("Directory", true);

            if (chageDirectory.GetSubKeyNames().All(r => r != "shell")) { return; }

            var chaveShell = chageDirectory.OpenSubKey("shell", true);

            if (chaveShell.GetSubKeyNames().Any(r => r == "Adicionar ao Jaime"))
                chaveShell.DeleteSubKeyTree("Adicionar ao Jaime");
        }

        private void CalcularDelay(int qtdCaracteres) {
            try {
                delay = int.Parse(decimal.Truncate(500 / qtdCaracteres).ToString());
            } catch (Exception ex) {
                delay = 500;
            }
        }

        void Pesquisar(object sender, EventArgs e) {
            var qtdCaracteres = txtPesquisar.Text.Length;

            CalcularDelay(qtdCaracteres);

            if (qtdCaracteres <= 1) {
                RevokeQueryTimer();
            } else {
                RestartQueryTimer();
            }
        }

        void RevokeQueryTimer() {
            if (_timer != null) {
                _timer.Stop();
                _timer.Tick -= queryTimer_Tick;
                _timer = null;
            }
        }

        void RestartQueryTimer() {
            if (_timer == null) {
                _timer = new System.Windows.Forms.Timer { Enabled = true, Interval = delay };
                _timer.Tick += queryTimer_Tick;
            } else {
                _timer.Stop();
                _timer.Start();
            }
        }

        void queryTimer_Tick(object sender, EventArgs e) {
            RevokeQueryTimer();
            var texto = txtPesquisar.Text.Replace("'", "%");
            if (texto.IsNullEmptyOrWhiteSpace() || texto.Length < 2) { return; }

            var lista = _formResultados.Controls["lbResultadosPesquisas"] as ListBox;

            foreach (var plugin in Plugins.Where(p => p.Tipo != (byte)TipoPlugin.Comando)) {
                var retornos = new string[0];
                var erros = new string[0];

                plugin.Filtrar(texto, out retornos, out erros);
                if (retornos.Any(r => !string.IsNullOrEmpty(r))) {
                    _iconePlugin = plugin.Icone;

                    _modoPlugin = true;
                    lista.DataSource = retornos.ToList();
                    _formResultados.Show();
                    ReposicionarCursor();
                    return;
                }

                if (erros.Any()) {
                    var mensagemErro = $"Ocorreram erros ao executar o plugin {plugin.Nome} {Environment.NewLine} Detalhes: {string.Join(Environment.NewLine, erros.Select(erro => erro).ToArray())}";
                    MessageBoxHelper.Error(mensagemErro);
                }
            }

            _modoPlugin = false;

            var favoritosEncontrados = FiltrarFavoritos(texto);

            if (!favoritosEncontrados.Any()) {
                _formResultados.Hide();
                TrocarIcone();
                return;
            }

            lista.DataSource = favoritosEncontrados.ToList();
            _formResultados.Show();

            Focus();
            txtPesquisar.Focus();
        }

        public void SetarTextoPesquisar(string texto) {
            txtPesquisar.Text = texto;
        }

        public string PegarTextoPesquisar() {
            return txtPesquisar.Text;
        }

        public void RemoverFavoritoMemoria(string nome) {
            _favoritosRegistro.Remove(nome);
        }

        public void AdicionarFavoritoMemoria(string nome) {
            _favoritosRegistro.Add(nome);
        }

        public void IncluirHistorico(string favorito) {
            if (HistoricoFavoritos.Any(h => h.Favorito.StartsWith(favorito))) { return; }

            HistoricoFavoritos.Add(new HistoricoFavoritoModel {
                Posicao = HistoricoFavoritos.Count + 1,
                Favorito = favorito
            });
        }

        private void LimparSelecaoHistorico() {
            foreach (var historico in HistoricoFavoritos.Where(h => h.Selecionado)) {
                historico.Selecionado = false;
            }

            txtPesquisar.Text = string.Empty;
            _formResultados.LimparLista();
            _formResultados.Hide();
            TrocarIcone();
        }

        private void SelecionarHistoricoFavorito() {
            if (!HistoricoFavoritos.Any()) { return; }

            var ultimaPosicaoSelecionada = HistoricoFavoritos.Where(h => h.Selecionado).Select(h => h.Posicao).FirstOrDefault();
            HistoricoFavoritoModel favoritoSelecionar;

            if (ultimaPosicaoSelecionada == 0 || ultimaPosicaoSelecionada == 1) {
                LimparSelecaoHistorico();

                favoritoSelecionar = HistoricoFavoritos.OrderBy(h => h.Posicao).Last();
                favoritoSelecionar.Selecionado = true;
                txtPesquisar.Text = favoritoSelecionar.Favorito;

                ReposicionarCursor();

                return;
            }

            var posicaoSelecionar = ultimaPosicaoSelecionada - 1;
            var qtdHistoricos = HistoricoFavoritos.Count;

            if (posicaoSelecionar <= 0) { posicaoSelecionar = qtdHistoricos; }

            favoritoSelecionar = HistoricoFavoritos.FirstOrDefault(p => p.Posicao == posicaoSelecionar);

            LimparSelecaoHistorico();

            if (favoritoSelecionar == null) { return; }

            favoritoSelecionar.Selecionado = true;
            txtPesquisar.Text = favoritoSelecionar.Favorito;

            ReposicionarCursor();
        }

        private void txtPesquisar_KeyDown(object sender, KeyEventArgs e) {
            var texto = ModoDiretorio || ModoArquivo || _primeiroFavoritoDaLista.IsNullEmptyOrWhiteSpace()
                ? txtPesquisar.Text
                : _primeiroFavoritoDaLista;

            if (e.KeyCode == Keys.Enter && !texto.IsNullEmptyOrWhiteSpace()) {
                // Necessário para parar o Ding do ENTER
                e.Handled = e.SuppressKeyPress = true;

                if (ModoDiretorio) {
                    if (Directory.Exists(texto)) {
                        if (!e.Control) {
                            txtPesquisar.Text = texto + @"\";
                            ReposicionarCursor();
                            return;
                        }

                        IncluirHistorico(txtPesquisar.Text);
                        Process.Start(txtPesquisar.Text);

                        Minimizar();
                        return;
                    }

                    MessageBoxHelper.Error("Não foi possível encontrar o caminho expecificado.");
                    return;
                }

                txtPesquisar.Enabled = false;
                var functions = new FavoritoService(this);
                string comandoRetorno;

                //Verifica se o texto inserido é um dos comandos configurados no sistema.
                if (!functions.AbrirComandoConfigurado(texto, out comandoRetorno)) {
                    //Caso contrário, inicia a operação de abrir o favorito.

                    bool hideForm;
                    if (functions.AbrirFavorito(texto, txtPesquisar.Text, e.Control, out hideForm)) {
                        IncluirHistorico(txtPesquisar.Text);

                        if (hideForm) {
                            _formResultados.Hide();
                            Minimizar();
                            _mostrar = false;
                        } else {
                            ReposicionarCursor();
                        }
                    }
                } else {
                    IncluirHistorico(comandoRetorno);
                    _formResultados.Hide();
                    Minimizar();
                    _mostrar = false;
                }

                txtPesquisar.Enabled = true;
            }
        }

        // Necessário para parar o Ding do ENTER
        private void txtPesquisar_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Enter) {
                e.Handled = true;
            }
        }
    }
}
