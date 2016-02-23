using System;
using System.Drawing;
using System.Globalization;
using JaimeCalcularExpressoesPlugin.Properties;
using JaimePlugin;
using NCalc;

namespace JaimeCalcularExpressoesPlugin {
    public class JaimeCalcularExpressoesPlugin : IJaimePlugin {
        public string Autor => "Leandro Simões";
        public string Site => @"https://www.facebook.com/Simoes.Leandro.Silva";
        public Image Icone => Resources.Expressoes;
        public string Nome => "Calcular expressões";
        public string Comando => string.Empty;
        public byte Tipo => 2;

        public void Filtrar(string textoEntrada, out string[] retornos, out string[] erros) {
            retornos = new string[0];
            erros = new string[0];

            retornos = new[] { Calcular(textoEntrada) };
        }

        public bool Executar(string textoEntrada, out string[] retornos, out string[] erros, out string retornoTextBox, out bool hideForm) {
            retornos = new string[0];
            erros = new string[0];
            hideForm = false;
            retornoTextBox = Calcular(textoEntrada);

            return !string.IsNullOrEmpty(retornoTextBox);
        }

        private string Calcular(string textoEntrada) {
            try {
                var expressao = textoEntrada.Trim();
                var expressaoModificada = expressao;

                char[] caracteres = { '/', '-', ' ', '!', '@', '#', '%', '¨', '&', '*', '(', ')', '_', '-', '+', '+' };

                foreach (var caractere in caracteres) {
                    expressaoModificada = expressaoModificada.Replace(caractere.ToString(), " ");
                }

                var numeros = expressaoModificada.Split(' ');
                foreach (var numero in numeros) {
                    if (string.IsNullOrEmpty(numero)) { continue; }

                    var n = double.Parse(numero, CultureInfo.GetCultureInfo("pt-br").NumberFormat).ToString().Replace(",", ".");
                    expressao = expressao.Replace(numero, n);
                }

                var e = new Expression(expressao);

                if (e.HasErrors()) { throw new Exception(); }

                return e.Evaluate().ToString();
            } catch (Exception ex) {
                return string.Empty;
            }
        }
    }
}
