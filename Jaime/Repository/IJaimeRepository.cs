using Jaime.Models;

namespace Jaime.Repository.Interface {
    public interface IJaimeRepository {
        FavoritoModel[] ObterFavoritos();
        ComandoModel[] ObterComandos(bool somenteComandosAtivos = false);
        ConfiguracaoModel[] ObterConfiguracoes();
        FavoritoModel[] ObterFavoritosEspecificos(string nome);
        MotorBuscaModel[] ObterMotoresBuscas();
        MotorBuscaModel[] ObterMotoresBuscasEspecificos(string comando);
        void SalvarMotorBusca(MotorBuscaModel motorBusca);
        void ExcluirMotorBusca(int id);
        void SalvarFavoritos(FavoritoModel[] favoritos);
        bool SalvarConfiguracoes(ConfiguracaoModel configuracoes);
        bool RenomearFavoritos(string novoCaminho, string novoNome, FavoritoModel[] favoritos, FavoritoModel favoritoPrincipal);
        bool DeletarFavoritos(string caminho);
        bool LimparFavoritos();
        bool DeletarConfiguracoes();
        bool AtualizarFavoritosSistema(FavoritoModel[] novosFavoritos);
        void CriarBanco();
        void DeletarBanco();
    }
}