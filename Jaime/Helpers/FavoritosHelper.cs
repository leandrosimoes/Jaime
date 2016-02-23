using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Jaime.Controls;
using Jaime.Enums;
using Jaime.Extensions;
using Jaime.Models;
using Jaime.Repository;

namespace Jaime.Helpers {
    public class FavoritosHelper {
        private string _nomeDocumento = "Documento1";
        private readonly JaimeRepository _repository = new JaimeRepository();
        private List<FavoritoModel> _favoritosExistentes = new List<FavoritoModel>();

        public bool RegistrarFavorito(string path) {
            var favoritoExistente = _repository.ObterFavoritos().FirstOrDefault(f => f.Caminho == path);

            if (favoritoExistente != null) {
                MessageBoxHelper.Error($"Você já adicionou este favorito com o nome: \"{favoritoExistente.Nome}\".");
                return false;
            }

            _nomeDocumento = PegarNomeArquivo(path);
            if (InputBox.Show("Adicionar ao Jaime", "Digite o nome do arquivo/pasta", ref _nomeDocumento) == DialogResult.OK) {
                if (_nomeDocumento.IsNullEmptyOrWhiteSpace()) {
                    MessageBoxHelper.Error("O nome do arquivo/pasta deve ser preenchido!");
                    return false;
                }

                Registrar(path, _nomeDocumento);
            }

            return false;
        }

        // Pega o nome do arquivo e joga no txtNome. Esta função foi criada pois alguns arquivos não tem
        // o nome de versão, então ele pega diretamente do arquivo. Mesmo assim o sistema joga o nome como
        // sugestão, podendo ser alterado pelo usuario.
        string PegarNomeArquivo(string path) {
            try {
                var ehDiretorio = Directory.Exists(path);
                var ehArquivo = File.Exists(path);

                var descricao = path.Split('\\').LastOrDefault();

                if (ehDiretorio || !ehArquivo) return descricao;

                var informacoesDeVersaoDoArquivo = FileVersionInfo.GetVersionInfo(path);
                var informacoesDoArquivo = new FileInfo(path);
                var nomeArquivo = informacoesDeVersaoDoArquivo.FileDescription;

                if (nomeArquivo.IsNullEmptyOrWhiteSpace())
                    nomeArquivo = informacoesDoArquivo.FullName.Split('\\').LastOrDefault();

                descricao = nomeArquivo.IsNullEmptyOrWhiteSpace() ? descricao : nomeArquivo;

                return descricao ?? _nomeDocumento;
            } catch (Exception) {
                // Se der erro traz o nome padrão
                return _nomeDocumento;
            }
        }

        private List<FavoritoModel> MontarFavoritosDiretorios(string favorito) {
            if (!Directory.Exists(favorito)) return null;

            return GetDirectoryFiles(favorito);
        }

        private List<FavoritoModel> GetDirectoryFiles(string path, string pattern = "*.*") {
            var files = new List<FavoritoModel>();

            try {
                foreach (var file in Directory.EnumerateFiles(path, pattern, SearchOption.TopDirectoryOnly)) {
                    var nome = Path.GetFileName(file);

                    var quantidadeNomeExistente = _favoritosExistentes.Count(f => f.Nome == nome || f.Nome.StartsWith(nome + " #"));

                    if (quantidadeNomeExistente > 0) nome = nome + " #" + quantidadeNomeExistente;

                    var favoritoAdicionar = new FavoritoModel {
                        Caminho = file,
                        Nome = nome,
                        Tipo = TipoFavorito.Arquivo,
                        Principal = false
                    };

                    files.Add(favoritoAdicionar);

                    _favoritosExistentes.Add(favoritoAdicionar);
                }
            } catch (UnauthorizedAccessException) { }

            return files;
        }

        public void Registrar(string path, string nomeFavorito) {
            _favoritosExistentes = _repository.ObterFavoritos().ToList();

            try {
                nomeFavorito = nomeFavorito.IsNullEmptyOrWhiteSpace() ? Path.GetFileName(path) : nomeFavorito;

                var quantidadeExistenteMesmoNome = _favoritosExistentes.Count(f => f.Nome == nomeFavorito || f.Nome.StartsWith(nomeFavorito + " #"));

                if (quantidadeExistenteMesmoNome > 0) nomeFavorito = nomeFavorito + " #" + quantidadeExistenteMesmoNome;

                var listaAdicionar = new List<FavoritoModel>();
                if (Directory.Exists(path)) {
                    if (_favoritosExistentes.All(f => f.Caminho != path)) {
                        var favoritoIncluir = new FavoritoModel {
                            Caminho = path,
                            Nome = nomeFavorito,
                            Tipo = TipoFavorito.Diretorio,
                            Principal = true
                        };

                        listaAdicionar.Add(favoritoIncluir);

                        _favoritosExistentes.Add(favoritoIncluir);
                    }

                    if (MessageBoxHelper.Question("Deseja adicionar também os arquivos deste diretório à lista de favoritos?") == DialogResult.Yes)
                        listaAdicionar.AddRange(MontarFavoritosDiretorios(path));
                }

                if (File.Exists(path)) {
                    if (_favoritosExistentes.All(f => f.Caminho != path)) {
                        var favoritoIncluir = new FavoritoModel {
                            Caminho = path,
                            Nome = nomeFavorito,
                            Tipo = TipoFavorito.Arquivo,
                            Principal = true
                        };

                        listaAdicionar.Add(favoritoIncluir);

                        _favoritosExistentes.Add(favoritoIncluir);
                    }
                }

                var t = new Thread(delegate () { _repository.SalvarFavoritos(listaAdicionar.ToArray()); });
                t.Start();

            } catch (Exception ex) {
                MessageBoxHelper.Error(ex.Message);
            }
        }
    }
}