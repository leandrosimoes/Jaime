using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using Jaime.Extensions;
using Jaime.Helpers;
using Jaime.Models;
using Jaime.Repository;
using Jaime.Enums;
using Microsoft.Win32;

namespace Jaime.Service {
    public class FavoritoService {
        private readonly Principal _frmPrincipal;
        private readonly JaimeRepository _repository = new JaimeRepository();
        public FavoritoService(Principal form) {
            _frmPrincipal = form;
        }

        public bool AbrirFavorito(string nome, string nomeDigitado, bool controlPressed, out bool hideForm) {
            hideForm = true;

            try {
                foreach (var plugin in _frmPrincipal.Plugins.Where(p => p.Tipo != (byte)TipoPlugin.Filtro)) {
                    var retornos = new string[0];
                    var erros = new string[0];
                    var retornoTextBox = string.Empty;

                    if (plugin.Executar(nomeDigitado, out retornos, out erros, out retornoTextBox, out hideForm)) {
                        if (erros.Any()) {
                            var mensagemErro = $"Ocorreram erros ao executar o plugin {plugin.Nome} {Environment.NewLine} Detalhes: {string.Join(Environment.NewLine, erros.Select(erro => erro).ToArray())}";
                            throw new Exception(mensagemErro);
                        }

                        if (!string.IsNullOrEmpty(retornoTextBox)) {
                            _frmPrincipal.SetarTextoPesquisar(retornoTextBox);
                        }
                        return true;
                    }
                };

                // kkk
                if (EasterEggs(nome))
                    return true;

                if (Directory.Exists(nome)) {
                    Process.Start(nome);
                    return true;
                }

                if (File.Exists(nome)) {
                    Process.Start(nome);
                    return true;
                }

                var favorito = _repository.ObterFavoritosEspecificos(nome).FirstOrDefault(n => n.Nome.ToLower() == nome.ToLower());

                if (favorito != null) {
                    if (Directory.Exists(favorito.Caminho)) {
                        _repository.SetarQuantidadeVezesAberto(favorito, nomeDigitado);
                        Process.Start(favorito.Caminho);
                        return true;
                    } else if (File.Exists(favorito.Caminho)) {
                        var caminho = favorito.Caminho;

                        if (controlPressed) {
                            var info = new FileInfo(caminho);
                            caminho = info.Directory?.ToString() ?? caminho;
                        }

                        _repository.SetarQuantidadeVezesAberto(favorito, nomeDigitado);
                        Process.Start(caminho);
                        return true;
                    } else {
                        MessageBoxHelper.Error("O favorito escolhido já não existe mais no caminho expecificado e será excluído do banco de dados.");
                        _repository.DeletarFavoritos(favorito.Caminho);
                        return false;
                    }
                }

                var favoritoNavegador = _frmPrincipal.FavoritosChrome.FirstOrDefault(fn => fn.Name == nome)?.Url ??
                                    _frmPrincipal.FavoritosFirefox.FirstOrDefault(fn => fn.title == nome)?.urlSemProtocolo;



                if (!favoritoNavegador.IsNullEmptyOrWhiteSpace()) {
                    var http = favoritoNavegador.StartsWith("http://") || favoritoNavegador.StartsWith("https://")
                                ? string.Empty
                                : "http://";

                    AbrirNoBrowserPadrao(http + favoritoNavegador);

                    return true;
                }

                var motorBusca = _repository.ObterMotoresBuscas().FirstOrDefault(f => f.Comando.StartsWith(nome.Split(' ').FirstOrDefault() ?? string.Empty));
                if (motorBusca != null) {
                    nome = nome.Replace(motorBusca.Comando, "");

                    var parametrosExistentes = motorBusca.Parametros.Split(';').Where(p => !string.IsNullOrEmpty(p)).ToArray();

                    var http = motorBusca.Url.StartsWith("http://") || motorBusca.Url.StartsWith("https://")
                        ? string.Empty
                        : "http://";

                    string url = string.Empty;
                    if (parametrosExistentes.Any()) {
                        url = http + string.Format(motorBusca.Url.Replace("{term}", Uri.EscapeDataString(nome)), parametrosExistentes);
                        AbrirNoBrowserPadrao(url);
                    } else {
                        url = http + motorBusca.Url.Replace("{term}", Uri.EscapeDataString(nome));
                        AbrirNoBrowserPadrao(url);
                    }

                    return true;
                }

                UltimosProcessos(nome);
            } catch (Exception ex) {
                MessageBoxHelper.Error(ex.Message);
                return false;
            }

            return true;
        }

        public static string GetDefaultBrowserPath() {
            const string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            const string browserPathKey = @"$BROWSER$\shell\open\command";

            try {
                var userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                if (userChoiceKey == null) {
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false) ??
                                     Registry.CurrentUser.OpenSubKey(urlAssociation, false);

                    var path = browserKey?.GetValue(null).ToString() ?? string.Empty;
                    browserKey?.Close();
                    return path;
                }

                var progId = (userChoiceKey.GetValue("ProgId").ToString());
                userChoiceKey.Close();

                var concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                var browserPath = kp?.GetValue(null).ToString() ?? string.Empty;
                kp.Close();
                return browserPath;
            } catch (Exception ex) {
                return string.Empty;
            }
        }

        private void AbrirNoBrowserPadrao(string url) {
            try {
                var path = GetDefaultBrowserPath();

                if (!string.IsNullOrEmpty(path)) {
                    Process.Start(path.Split('-').FirstOrDefault()?.Trim() ?? "iexplore.exe", url);
                    return;
                }

                throw new Exception("Nenhum browser padrão encontrado.");
            } catch (Exception ex) {
                throw new Exception($"Não foi possível abrir a url solicitada. {Environment.NewLine}{Environment.NewLine} Detalhes: {ex.Message}");
            }
        }

        private void UltimosProcessos(string favorito, int qtdTentativas = 1) {
            try {
                if (qtdTentativas == 1) {
                    Process.Start(favorito);
                } else {
                    var configuracoes = _repository.ObterConfiguracoes().First();
                    var httpProxy = new HttpProxyModel();
                    string formatedUrl;
                    if (configuracoes.Proxy) {
                        httpProxy.Enabled = true;
                        httpProxy.Server = configuracoes.ProxyServidor;
                        httpProxy.Port = Convert.ToInt32(configuracoes.ProxyPorta);
                        httpProxy.UserName = configuracoes.ProxyUsuario;
                        httpProxy.Password = configuracoes.ProxySenha;
                    }

                    if (HttpRequestHelper.CreateGetHttpResponse(favorito, httpProxy, out formatedUrl) != null) {
                        if (formatedUrl.IsValidUri()) {
                            AbrirNoBrowserPadrao(formatedUrl);
                        } else {
                            throw new Exception("Não foi possível encontrar o arquivo/comando.");
                        }
                    }
                }
            } catch (Exception ex) {
                if (qtdTentativas <= 0) {
                    MessageBoxHelper.Error($"Não foi possível executar o comando: \"{favorito}\". {Environment.NewLine}{Environment.NewLine} Detalhes: {ex.Message}.");
                    return;
                }

                qtdTentativas = qtdTentativas - 1;
                UltimosProcessos(favorito, qtdTentativas);
            }
        }

        private bool EasterEggs(string favorito) {
            if (!favorito.StartsWith("o menino está com sede"))
                return false;

            AbrirNoBrowserPadrao("https://www.youtube.com/watch?v=_Bw0RVwFGPM");

            return true;
        }

        public bool AbrirComandoConfigurado(string comando, out string comandoRetorno) {
            comandoRetorno = string.Empty;

            var comandoEncontrado = _repository.ObterComandos(true).FirstOrDefault(c => c.name.StartsWith(comando.Split(' ').FirstOrDefault() ?? string.Empty));

            comandoEncontrado = comando.StartsWith("> ") ? new ComandoModel { name = "> " } : comandoEncontrado;
            comandoEncontrado = comando.StartsWith("config") ? new ComandoModel { name = "config" } : comandoEncontrado;
            comandoEncontrado = comando.StartsWith("close") ? new ComandoModel { name = "close" } : comandoEncontrado;

            if (comandoEncontrado == null) { return false; }

            comandoEncontrado.name = comandoEncontrado.name.Replace("Comando", "");

            if (comandoEncontrado.name == "> ") {
                comandoRetorno = comandoEncontrado.name;
                ExecutarComandoCmd(comando, true);
            } else if (comandoEncontrado.name == "config") {
                comandoRetorno = comandoEncontrado.name;
                _frmPrincipal.AbrirJanelaConfiguracoes();
            } else if (comandoEncontrado.name == "close") {
                comandoRetorno = comandoEncontrado.name;
                _frmPrincipal.FecharJaime();
            } else {
                return false;
            }

            return true;
        }

        public bool VerificarComandoConfigurado(string favorito) {
            var comandos = _repository.ObterComandos(true);
            var comandoEncontrado = comandos.FirstOrDefault(c => c.name.StartsWith(favorito.Split(' ').FirstOrDefault()));

            comandoEncontrado = favorito.StartsWith("> ") ? new ComandoModel { name = "> " } : comandoEncontrado;
            comandoEncontrado = favorito.StartsWith("config") ? new ComandoModel { name = "config" } : comandoEncontrado;
            comandoEncontrado = favorito.StartsWith("close") ? new ComandoModel { name = "close" } : comandoEncontrado;

            if (comandoEncontrado == null) { return false; }

            comandoEncontrado.name = comandoEncontrado.name.Replace("Comando", "");

            var comandosValidos = new[] {
                "CMD",
                "cmd",
                "config",
                "close"
            };

            return comandosValidos.Any(c => c == comandoEncontrado.name);
        }

        public bool ExecutarComandoCmd(string cmd, bool runAsAdministrator = false) {
            if (!cmd.StartsWith("> ")) { return false; }

            cmd = cmd.Replace("> ", "");

            try {
                if (string.IsNullOrEmpty(cmd))
                    throw new ArgumentNullException();

                WindowsShellRun.Start(cmd, runAsAdministrator);
                return true;
            } catch (Exception) {
                MessageBoxHelper.Error($"Não foi possível executar o comando \"{cmd}\".");
            }
            return false;
        }
    }
}