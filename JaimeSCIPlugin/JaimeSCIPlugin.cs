using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using JaimePlugin;
using JaimeSCIPlugin.Properties;

namespace JaimeSCIPlugin {
    public class JaimeSCIPlugin : IJaimePlugin {
        public string Autor => "Leandro Simões";
        public string Site => @"https://www.facebook.com/Simoes.Leandro.Silva";
        public Image Icone => Resources.sci;
        public string Nome => "SimplificaCI Command Line App Plugin";
        public string Comando => "sci ";
        public byte Tipo => 0;

        public void Filtrar(string textoEntrada, out string[] retornos, out string[] erros) { throw new NotImplementedException(); }

        public bool Executar(string textoEntrada, out string[] retornos, out string[] erros, out string retornoTextBox, out bool hideForm) {
            retornos = new string[0];
            erros = new string[0];
            retornoTextBox = string.Empty;
            hideForm = true;

            try {
                if (!textoEntrada.StartsWith(Comando)) return false;

                Process.Start("CMD.exe", $"/k {textoEntrada}");

                return true;
            } catch (Exception ex) {
                erros = new[] { ex.Message };
                return false;
            }
        }
    }
}