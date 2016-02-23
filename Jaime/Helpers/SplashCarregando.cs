using System.Threading;

namespace Jaime.Helpers {
    public static class SplashCarregando {
        static Carregando _formCarregando = null;

        public static void MostrarSplashCarregando() {
            if (_formCarregando != null) return;

            _formCarregando = new Carregando();
            _formCarregando.MostarSplashCarregando();
        }

        public static void FecharSplashCarregando() {
            if (_formCarregando == null) return;

            _formCarregando.FecharSplashCarregando();
            _formCarregando = null;
        }

        public static void MudarMensagemStatus(string Text, int millisegundos = 1000) {
            if (_formCarregando != null)
                _formCarregando.MudarMensagemStatus(Text);

            Thread.Sleep(millisegundos);
        }
    }

}
