using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using Jaime.Enums;
using Jaime.Helpers;
using Jaime.Models;
using Jaime.Repository.Interface;

namespace Jaime.Repository
{
    public class JaimeRepository : IJaimeRepository {
        public static string DbFolder => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\JaimeFavoritos\\";
        public static string DbFile => DbFolder + "\\JaimeFavoritos.sqlite";

        public FavoritoModel[] ObterFavoritos() {
            if (!File.Exists(DbFile)) { return null; }

            using (var cnn = new SQLiteConnection("DataSource = " + DbFile)) {
                cnn.Open();
                var retorno = cnn.Query<FavoritoModel>(
                   "SELECT * FROM Favoritos"
                ).ToArray();
                cnn.Close();

                return retorno;
            }
        }

        public ConfiguracaoModel[] ObterConfiguracoes() {
            if (!File.Exists(DbFile)) { return null; }

            using (var cnn = new SQLiteConnection("DataSource = " + DbFile)) {
                cnn.Open();
                var retorno = cnn.Query<ConfiguracaoModel>(
                    "SELECT * FROM Configuracoes"
                    ).ToArray();
                cnn.Close();

                return retorno;
            }
        }

        public ComandoModel[] ObterComandos(bool somenteComandosAtivos = false) {
            if (!File.Exists(DbFile)) { return null; }

            using (var cnn = new SQLiteConnection("DataSource = " + DbFile)) {
                cnn.Open();
                var comandos = cnn.Query<ComandoModel>(
                    "PRAGMA table_info(Configuracoes);"
                    ).Where(c => c.name.StartsWith("Comando")).ToArray();

                var retorno = new List<ComandoModel>();

                foreach (var comando in comandos) {
                    retorno.AddRange(cnn.Query<ComandoModel>(
                        "SELECT " + comando.name + " AS name FROM Configuracoes" + (somenteComandosAtivos ? " WHERE " + comando.name.Replace("Comando", "") + " = 1" : string.Empty)
                    ).ToArray());
                }
                cnn.Close();

                return retorno.ToArray();
            }
        }

        public FavoritoModel[] ObterFavoritosEspecificos(string nome) {
            if (!File.Exists(DbFile)) { return null; }

            using (var cnn = new SQLiteConnection("DataSource = " + DbFile)) {
                cnn.Open();
                var retorno = cnn.Query<FavoritoModel>(
                   "SELECT * FROM Favoritos WHERE Nome LIKE '%" + nome + "%' COLLATE NOCASE ORDER BY QuantidadeVezesAberto DESC"
                ).ToArray();
                cnn.Close();

                return retorno;
            }
        }

        public MotorBuscaModel[] ObterMotoresBuscas() {
            if (!File.Exists(DbFile)) { return null; }

            using (var cnn = new SQLiteConnection("DataSource = " + DbFile)) {
                cnn.Open();
                var retorno = cnn.Query<MotorBuscaModel>(
                   "SELECT * FROM MotoresBuscas"
                ).ToArray();
                cnn.Close();

                return retorno;
            }
        }

        public MotorBuscaModel[] ObterMotoresBuscasEspecificos(string comando) {
            if (!File.Exists(DbFile)) { return null; }

            using (var cnn = new SQLiteConnection("DataSource = " + DbFile)) {
                cnn.Open();
                var retorno = cnn.Query<MotorBuscaModel>(
                   "SELECT * FROM MotoresBuscas WHERE Comando = '" + comando + "' COLLATE NOCASE"
                ).ToArray();
                cnn.Close();

                return retorno;
            }
        }

        public bool SalvarConfiguracoes(ConfiguracaoModel configuracoes) {
            if (!DeletarConfiguracoes()) {
                throw new Exception("Não foi possível deletar as configurações.");
            }

            const string fields =
                "Opacidade, " +
                "FavoritosNavegadores, " +
                "IniciarComWindows, " +
                "CorFundo, " +
                "CorFonte, " +
                "Fonte, " +
                "SubstituirTeclaWindowsR, " +
                "Proxy, ProxyServidor, ProxyPorta, ProxyUsuario, ProxySenha, " +
                "DataUltimaAtualizacao";

            var values = "(" +
                    $"'{configuracoes.Opacidade}', " +
                    $"{(configuracoes.FavoritosNavegadores ? 1 : 0)}, " +
                    $"{(configuracoes.IniciarComWindows ? 1 : 0)}, " +
                    $"'{configuracoes.CorFundo}', " +
                    $"'{configuracoes.CorFonte}', " +
                    $"'{configuracoes.Fonte}', " +
                    $"{(configuracoes.SubstituirTeclaWindowsR ? 1 : 0)}, " +
                    $"{(configuracoes.Proxy ? 1 : 0)}, '{configuracoes.ProxyServidor}', '{configuracoes.ProxyPorta}', '{configuracoes.ProxyUsuario}', '{configuracoes.ProxySenha}', " +
                    $"'{configuracoes.DataUltimaAtualizacao}'" + 
                ");";

            ExecutarInsert(values, "Configuracoes", fields);

            return true;
        }

        public void SalvarFavoritos(FavoritoModel[] favoritos) {
            var count = 500;
            var total = favoritos.Count();

            if (total <= 0) { return; }

            var favoritosAdicionar = favoritos.Select(f => $"(\"{f.Nome}\", \"{f.Caminho}\", {(int)f.Tipo}, \"{f.Principal}\", {(int)0})").ToArray();
            var values = string.Empty;
            foreach (var favorito in favoritosAdicionar) {
                try {
                    count--;
                    total--;
                    values += favorito;

                    if (count > 0 && total > 0)
                        values += ", ";
                    else {
                        count = 500;
                        ExecutarInsert(values + ";", "Favoritos", "Nome, Caminho, Tipo, Principal, QuantidadeVezesAberto");
                        values = string.Empty;
                    }
                } catch (Exception ex) { }
            }
        }

        public void EditarFavoritos(FavoritoModel favorito) {
            try {
                var values = $@"Nome = '{favorito.Nome}', Caminho = '{favorito.Caminho}', Tipo = {(int)favorito.Tipo}, Principal = {(favorito.Principal ? 1 : 0)}, QuantidadeVezesAberto = {favorito.QuantidadeVezesAberto}, DataUltimaAbertura = '{favorito.DataUltimaAbertura}'";
                ExecutarUpdate(favorito.Id, values, "Favoritos");
            } catch (Exception ex) {
            }
        }

        public void SalvarMotorBusca(MotorBuscaModel motorBusca) {
            try {
                var values = $"(\"{motorBusca.Url}\", \"{motorBusca.Comando}\", \"{motorBusca.Parametros}\")";
                ExecutarInsert(values + ";", "MotoresBuscas", "Url, Comando, Parametros");
            } catch (Exception) { }
        }

        public void ExcluirMotorBusca(int id) {
            try {
                ExecutarDelete(id, "MotoresBuscas");
            } catch (Exception ex) { }
        }

        private static void ExecutarInsert(string values, string table, string fields) {
            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                cnn.Query(@"INSERT INTO " + table + " (" + fields + ") VALUES " + values);
                cnn.Close();
            }
        }

        private static void ExecutarUpdate(int id, string values, string table) {
            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                cnn.Query(@"UPDATE " + table + " SET " + values + " WHERE Id = " + id);
                cnn.Close();
            }
        }

        private static void ExecutarDelete(int id, string table) {
            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                cnn.Execute(@"DELETE FROM " + table + " WHERE Id = " + id);
                cnn.Close();
            }
        }

        public bool DeletarFavoritos(string caminho) {
            if (!File.Exists(DbFile)) {
                return true;
            }

            try {
                using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                    cnn.Open();
                    cnn.Execute("DELETE FROM Favoritos WHERE Caminho LIKE '" + caminho + "%'");
                    var qtd = cnn.Execute("SELECT COUNT(*) FROM Favoritos");

                    if (qtd == 0)
                        cnn.Execute("DELETE FROM sqlite_sequence WHERE Name = 'Favoritos'");
                    else {
                        qtd = cnn.Query<int>("SELECT MAX(Id) FROM Favoritos").FirstOrDefault();
                        cnn.Execute("UPDATE sqlite_sequence SET sqt = '" + (qtd + 1) + "' WHERE Name = 'Favoritos'");
                    }
                    cnn.Close();
                }

                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public bool DeletarConfiguracoes() {
            if (!File.Exists(DbFile)) {
                return true;
            }

            try {
                using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                    cnn.Open();
                    cnn.Execute("DELETE FROM Configuracoes");
                    cnn.Execute("DELETE FROM sqlite_sequence WHERE Name = 'Configuracoes'");
                    cnn.Close();
                }

                return true;
            } catch (Exception ex) {
                return false;
            }
        }

        public bool AtualizarFavoritosSistema(FavoritoModel[] novosFavoritos) {
            var favoritosAntigos = new FavoritoModel[0];
            var favoritosIncluir = new List<FavoritoModel>();
            var favoritosExcluir = new List<FavoritoModel>();

            try {
                using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                    cnn.Open();
                    favoritosAntigos = cnn.Query<FavoritoModel>("SELECT * FROM Favoritos WHERE Tipo = " + (int)TipoFavorito.Sistema).ToArray();

                    favoritosIncluir.AddRange(novosFavoritos.Where(novo => favoritosAntigos.All(f => f.Caminho != novo.Caminho && f.Nome != novo.Nome)));
                    favoritosExcluir.AddRange(favoritosAntigos.Where(ant => favoritosAntigos.All(f => f.Caminho != ant.Caminho && f.Nome != ant.Nome)));

                    foreach (var excluir in favoritosExcluir) {
                        DeletarFavoritos(excluir.Caminho);
                    }

                    var qtd = cnn.Query<int>("SELECT COUNT(*) FROM Favoritos").FirstOrDefault();

                    if (qtd == 0)
                        cnn.Execute("DELETE FROM sqlite_sequence WHERE Name = 'Favoritos'");
                    else {
                        var novaSequencia = cnn.Query<int>("SELECT MAX(Id) FROM Favoritos").FirstOrDefault();
                        cnn.Execute("UPDATE sqlite_sequence SET seq = '" + novaSequencia + "' WHERE Name = 'Favoritos'");
                    }

                    cnn.Close();
                }


                SalvarFavoritos(favoritosIncluir.ToArray());

                return true;
            } catch (Exception) {
                return false;
            }
        }

        public void CriarBanco() {
            if (!Directory.Exists(DbFolder)) {
                Directory.CreateDirectory(DbFolder);
            }

            if (File.Exists(DbFile)) return;

            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                try {
                    CriarTabelaFavoritos();
                    CriarTabelaConfiguracoes();
                    CriarTabelaMotoresBuscas();

                    cnn.Execute(@"
                        INSERT INTO MotoresBuscas (
                            Comando,
                            Url,
                            Parametros
                        ) VALUES 
                            ('google ', 'https://www.google.com.br/search?q={term}', ''),
                            ('youtube ', 'https://www.youtube.com/results?search_query={term}', ''),
                            ('traduzir ', 'http://www.google.com/translate_t?langpair=auto|{0}&text={term}', 'pt'),
                            ('stack ', 'http://stackoverflow.com/search?q={term}', '');
                    ");
                    cnn.Execute(@"
                            INSERT INTO Configuracoes (
                                    Opacidade,
                                    FavoritosNavegadores,
                                    IniciarComWindows,
                                    CorFundo,
                                    CorFonte,
                                    Fonte,
                                    SubstituirTeclaWindowsR,
                                    Proxy, ProxyServidor, ProxyPorta, ProxyUsuario, ProxySenha,
                                    DataUltimaAtualizacao
                            ) VALUES (
                                    '100',
                                    1,
                                    0,
                                    'Black',
                                    '255, 128, 0',
                                    'Arial',
                                    0,
                                    0, '', '', '', '',
                                    '" + $"{DateTime.Now:yyyy/MM/dd HH:mm:ss}" + @"'
                            );"
                    );
                } catch (Exception ex) {
                    MessageBoxHelper.Error($"Não foi possível criar o banco de dados, motivo: {Environment.NewLine + Environment.NewLine + ex.Message}");
                }
                cnn.Close();
            }
        }

        private void CriarTabelaFavoritos() {
            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                cnn.Execute(
                    @"
                        CREATE TABLE IF NOT EXISTS Favoritos
                        (
                            Id                    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            Caminho               VARCHAR(500),
                            Nome                  VARCHAR(100),
                            Tipo                  INTEGER DEFAULT(0),
                            Principal             BOOLEAN,
                            QuantidadeVezesAberto INTEGER DEFAULT(0),
                            DataUltimaAbertura    VARCHAR(100)
                        );"
                    );
                cnn.Close();
            }
        }

        private void CriarTabelaMotoresBuscas() {
            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                cnn.Execute(@"
                         CREATE TABLE IF NOT EXISTS MotoresBuscas
                        (
                            Id                    INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            Comando               VARCHAR(500),
                            Url                   VARCHAR(500),
                            Parametros            VARCHAR(500)
                        );"
                );
                cnn.Close();
            }
        }

        private void CriarTabelaConfiguracoes() {
            using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                cnn.Open();
                cnn.Execute(@"
                        CREATE TABLE IF NOT EXISTS Configuracoes
                        (
                            Id                              INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                            Opacidade                       VARCHAR(10),
                            FavoritosNavegadores            BOOLEAN,
                            IniciarComWindows               BOOLEAN,
                            CorFundo                        VARCHAR(100),
                            CorFonte                        VARCHAR(100),
                            Fonte                           VARCHAR(100),
                            SubstituirTeclaWindowsR         BOOLEAN,
                            Proxy                           BOOLEAN,
                            ProxyServidor                   VARCHAR(100),
                            ProxyPorta                      VARCHAR(100),
                            ProxyUsuario                    VARCHAR(100),
                            ProxySenha                      VARCHAR(100),
                            DataUltimaAtualizacao           VARCHAR(10)
                        );"
                );
                cnn.Close();
            }
        }

        public void DeletarBanco() {
            try {
                if (File.Exists(DbFile)) {
                    File.Delete(DbFile);
                }
            } catch (Exception ex) {
                MessageBoxHelper.Error($"Não foi possível deletar o banco de dados, motivo: {Environment.NewLine + Environment.NewLine + ex.Message}");
            }
        }

        public bool LimparFavoritos() {
            if (!File.Exists(DbFile)) {
                return true;
            }

            try {
                using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                    cnn.Open();
                    cnn.Execute("DELETE FROM Favoritos");
                    cnn.Execute("DELETE FROM sqlite_sequence WHERE Name = 'Favoritos'");
                    cnn.Close();
                }

                return true;
            } catch (Exception) {
                return false;
            }
        }

        public bool RenomearFavoritos(string novoCaminho, string novoNome, FavoritoModel[] favoritosAtualizar, FavoritoModel favoritoPrincipal) {
            try {
                using (var cnn = new SQLiteConnection("DataSource=" + DbFile)) {
                    cnn.Open();
                    cnn.Execute(string.Format("UPDATE Favoritos SET Caminho = \"{0}\", Nome = \"{1}\" WHERE Id = {2}", novoCaminho, novoNome, favoritoPrincipal.Id));
                    foreach (var favorito in favoritosAtualizar) {
                        if (favorito.Caminho == favoritoPrincipal.Caminho && favorito.Nome == favoritoPrincipal.Nome) { continue; }

                        var caminho = favorito.Caminho.Replace(favorito.Caminho.Split('\\').LastOrDefault(), "");
                        caminho = favorito.Caminho.Replace(caminho, novoCaminho + "\\");
                        cnn.Execute(string.Format("UPDATE Favoritos SET Caminho = \"{0}\" WHERE Id = {1}", caminho, favorito.Id));
                    }
                    cnn.Close();
                }

                return true;
            } catch (Exception) {
                return false;
            }
        }

        public void SetarQuantidadeVezesAberto(FavoritoModel favorito, string nome) {
            var favoritosEspecificos = ObterFavoritosEspecificos(nome).Where(f => f.Id != favorito.Id && !string.IsNullOrEmpty(f.DataUltimaAbertura)).ToArray();

            favorito.QuantidadeVezesAberto += 5;
            favorito.DataUltimaAbertura = DateTime.Now.ToString();
            EditarFavoritos(favorito);

            foreach (var favoritoEspecifico in favoritosEspecificos) {
                DateTime dataUltimaAbertura;
                DateTime.TryParse(favoritoEspecifico.DataUltimaAbertura, out dataUltimaAbertura);

                if (dataUltimaAbertura == DateTime.MinValue || dataUltimaAbertura.Date == DateTime.Today) continue;

                //A quantidade leva em consideração a quantos dias você não abre este favorito
                //Para cada dia, são removidos 5 pontos, ou seja, quanto mais dias fizer que este
                //favorito não é aberto, mais pontos serão subtraídos
                var quantidadeSubtrair = Math.Abs((DateTime.Now - dataUltimaAbertura).Days * 5);

                favoritoEspecifico.QuantidadeVezesAberto -= quantidadeSubtrair <= 0 ? 5 : quantidadeSubtrair;

                if (favoritoEspecifico.QuantidadeVezesAberto <= 0) {
                    favoritoEspecifico.QuantidadeVezesAberto = 0;
                    favoritoEspecifico.DataUltimaAbertura = string.Empty;
                }

                EditarFavoritos(favoritoEspecifico);
            }
        }
    }
}