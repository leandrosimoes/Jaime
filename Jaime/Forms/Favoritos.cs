using System;
using System.Linq;
using System.Windows.Forms;
using Jaime.Extensions;
using Jaime.Helpers;
using Jaime.Models;
using Jaime.Repository;

namespace Jaime {
    public partial class Favoritos : Form {
        private readonly JaimeRepository _repository = new JaimeRepository();
        private System.Windows.Forms.Timer _delayPesquisa;
        private bool _pesquisando;
        private readonly Principal _formPrincipal;

        public Favoritos(Principal formPrincipal) {
            _formPrincipal = formPrincipal;
            InitializeComponent();
        }

        private void btnExcluirFavoritos_Click(object sender, EventArgs e) {
            try {
                if (MessageBoxHelper.Alert("Tem certeza que deseja excluir os favoritos selecionados?") == DialogResult.No) { return; }

                var excluiuAlgum = false;
                var jaimeFavoritos = _repository.ObterFavoritos();
                if (jaimeFavoritos.Any()) {
                    foreach (DataGridViewRow row in gridFavoritos.Rows) {
                        var colunaSelecionar = row.Cells[0] as DataGridViewCheckBoxCell;
                        var colunaNome = row.Cells[1] as DataGridViewTextBoxCell;
                        var colunaCaminho = row.Cells[2] as DataGridViewTextBoxCell;
                        if ((bool)colunaSelecionar.Value) {
                            excluiuAlgum = true;
                            _repository.DeletarFavoritos(colunaCaminho.Value.ToString());
                            _formPrincipal.RemoverFavoritoMemoria(colunaNome.Value.ToString());
                        }
                    }
                }

                if (!excluiuAlgum)
                    throw new Exception("Você deve selecionar ao menos um favorito para excluir.");

                Close();
            } catch (Exception ex) {
                MessageBoxHelper.Error(ex.Message); ;
            }
        }

        private void txtPesquisar_TextChanged(object sender, EventArgs e) {
            RestartQueryTimer();
        }

        void RevokeQueryTimer() {
            if (_delayPesquisa != null) {
                _delayPesquisa.Stop();
                _delayPesquisa.Tick -= queryTimer_Tick;
                _delayPesquisa = null;
            }
        }

        void RestartQueryTimer() {
            if (_delayPesquisa == null) {
                _delayPesquisa = new System.Windows.Forms.Timer { Enabled = true, Interval = 500 };
                _delayPesquisa.Tick += queryTimer_Tick;
            } else {
                _delayPesquisa.Stop();
                _delayPesquisa.Start();
            }
        }

        void queryTimer_Tick(object sender, EventArgs e) {
            RevokeQueryTimer();
            var texto = txtPesquisar.Text;

            try {
                var jaimeFavoritos = new FavoritoModel[0];

                if (texto.Length > 2) {
                    _pesquisando = true;
                    jaimeFavoritos = _repository.ObterFavoritosEspecificos(texto);

                    lblQtdCaracteres.Text = string.Format("{0} Favorito(s) encontrado(s)", jaimeFavoritos.Count());
                } else {
                    lblQtdCaracteres.Text = string.Format("Digite {0} caracteres para pesquisar", (3 - texto.Length));

                    if (!_pesquisando) { return; }

                    jaimeFavoritos = _repository.ObterFavoritos();
                    _pesquisando = false;
                }

                if (jaimeFavoritos.Any()) {
                    gridFavoritos.Rows.Clear();

                    foreach (var favorito in jaimeFavoritos) {
                        if (favorito.Nome == null) {
                            continue;
                        }

                        gridFavoritos.Rows.Add(false, favorito.Nome, favorito.Caminho);
                    }

                    gridFavoritos.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    gridFavoritos.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                } else {
                    lblQtdCaracteres.Text = "Nenhum favorito encontrado";
                    gridFavoritos.Rows.Clear();
                }
            } catch (Exception ex) {
                MessageBoxHelper.Error(ex.Message);
            }

            Focus();
            txtPesquisar.Focus();
        }

        private void Favoritos_Load(object sender, EventArgs e) {
            lblQtdCaracteres.Text = "Digite 3 caracteres para pesquisar";
            txtPesquisar.Text = string.Empty;
            txtPesquisar.Focus();
        }

        private void btnSelecao_Click(object sender, EventArgs e) {
            foreach (DataGridViewRow row in gridFavoritos.Rows) {
                var cell = (DataGridViewCheckBoxCell)row.Cells[0];
                cell.Value = !(bool)cell.Value;
            }
        }
    }
}
