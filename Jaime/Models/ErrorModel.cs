namespace Jaime.Models {
    public class ErrorModel {
        public ErrorModel(string local, string mensagem) {
            Local = local;
            Mensagem = mensagem;
            Detalhes = string.Empty;
        }

        public ErrorModel(string local, string mensagem, string detalhes) {
            Local = local;
            Mensagem = mensagem;
            Detalhes = detalhes;
        }

        public string Local { get; set; }
        public string Mensagem { get; set; }
        public string Detalhes { get; set; }
    }
}