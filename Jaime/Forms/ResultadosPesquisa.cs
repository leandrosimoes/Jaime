using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Jaime.Extensions;
using System.Linq;
using Jaime.Models;
using Jaime.Repository;
using Jaime.Service;

namespace Jaime {
    public partial class ResultadosPesquisa : Form {
        private readonly Principal _formPrincipal;
        private readonly JaimeRepository _repository = new JaimeRepository();

        public ResultadosPesquisa(Principal form) {
            InitializeComponent();

            _formPrincipal = form;
            Opacity = Double.Parse(_repository.ObterConfiguracoes().First().Opacidade.ToString());
        }

        private void lbResultadosPesquisas_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Back) { _formPrincipal.Focus(); }

            if (e.KeyCode == Keys.Up) {
                if (lbResultadosPesquisas.SelectedIndex == 0) {
                    _formPrincipal.Focus();
                }
            }

            if (e.KeyCode == Keys.Escape) {
                _formPrincipal.Focus();
            }

            if (e.KeyCode == Keys.Enter) {
                var texto = lbResultadosPesquisas.SelectedValue.ToString();
                var textoDigitado = _formPrincipal.PegarTextoPesquisar();

                if (_formPrincipal.ModoDiretorio || _formPrincipal.ModoArquivo) {
                    if (Directory.Exists(texto)) {
                        _formPrincipal.SetarTextoPesquisar(texto + "\\");
                        _formPrincipal.ReposicionarCursor();
                    } else {
                        _formPrincipal.IncluirHistorico(texto);
                        Process.Start(texto);
                        _formPrincipal.Minimizar();
                        Hide();
                    }

                    return;
                }

                var functions = new FavoritoService(_formPrincipal);
                string comandoRetorno;

                //Verifica se o texto inserido é um dos comandos configurados no sistema.
                if (!functions.AbrirComandoConfigurado(texto, out comandoRetorno)) {
                    bool hideForm;

                    //Caso contrário, inicia a operação de abrir o favorito.
                    if (functions.AbrirFavorito(texto, textoDigitado, e.Control, out hideForm)) {
                        _formPrincipal.IncluirHistorico(textoDigitado);

                        if (hideForm) {
                            _formPrincipal.Minimizar();
                            Hide();
                        }
                    }

                    Hide();
                } else {
                    _formPrincipal.IncluirHistorico(comandoRetorno);

                    if (comandoRetorno.IsNullEmptyOrWhiteSpace()) {
                        _formPrincipal.Minimizar();
                    } else {
                        _formPrincipal.ReposicionarCursor();
                    }

                    Hide();
                }
            }
        }

        private void lbResultadosPesquisas_Click(object sender, EventArgs e) {
            var texto = lbResultadosPesquisas.SelectedValue.ToString();
            var functions = new FavoritoService(_formPrincipal);
            string comandoRetorno;

            //Verifica se o texto inserido é um dos comandos configurados no sistema.
            if (!functions.AbrirComandoConfigurado(texto, out comandoRetorno)) {
                var textoDigitado = _formPrincipal.PegarTextoPesquisar();

                _formPrincipal.IncluirHistorico(textoDigitado);

                bool hideForm;

                //Caso contrário, inicia a operação de abrir o favorito.
                if (functions.AbrirFavorito(texto, textoDigitado, false, out hideForm)) {
                    if (hideForm) {
                        _formPrincipal.Minimizar();
                        Hide();
                    }
                }

                Hide();
            } else {
                _formPrincipal.IncluirHistorico(comandoRetorno);

                if (comandoRetorno.IsNullEmptyOrWhiteSpace()) {
                    _formPrincipal.Minimizar();
                } else {
                    _formPrincipal.ReposicionarCursor();
                }

                Hide();
            }
        }

        private void lbResultadosPesquisas_SelectedValueChanged(object sender, System.EventArgs e) {
            var favorito = lbResultadosPesquisas.SelectedItem != null
                ? lbResultadosPesquisas.SelectedItem.ToString()
                : string.Empty;

            _formPrincipal.TrocarIconeAoMoverLista(favorito);
        }

        private void ResultadosPesquisa_Shown(object sender, System.EventArgs e) {
            _formPrincipal.AtualizarPosicaoLista();
        }

        public void LimparLista() {
            lbResultadosPesquisas.DataSource = null;
        }
    }
}
