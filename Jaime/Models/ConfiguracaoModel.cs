namespace Jaime.Models {
    public class ConfiguracaoModel {
        public int Id { get; set; }
        public string Opacidade { get; set; }
        public bool FavoritosNavegadores { get; set; }
        public bool IniciarComWindows { get; set; }
        public string CorFundo { get; set; }
        public string CorFonte { get; set; }
        public string Fonte { get; set; }
        public bool SubstituirTeclaWindowsR { get; set; }
        public bool Proxy { get; set; }
        public string ProxyServidor { get; set; }
        public string ProxyPorta { get; set; }
        public string ProxyUsuario { get; set; }
        public string ProxySenha { get; set; }
        public string DataUltimaAtualizacao { get; set; }
    }
}