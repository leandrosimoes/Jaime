using System.Drawing;
using System.Collections.Generic;

namespace JaimePlugin {
    public interface IJaimePlugin {
        /// <summary>
        /// Tipo do plugin deve ser:
        /// 
        /// 0 - Comando
        /// 1 - Filtro
        /// 2 - Híbrido
        /// </summary>
        byte Tipo { get; }

        string Nome { get; }
        string Autor { get; }
        string Site { get; }
        Image Icone { get; }

        string Comando { get; }

        bool Executar(string textoEntrada, out string[] retornos, out string[] erros, out string retornoTextBox, out bool hideForm);
        void Filtrar(string textoEntrada, out string[] retornos, out string[] erros);
    }
}