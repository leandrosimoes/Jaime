using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using JaimeCriarNotasPlugin.Controls;
using JaimeCriarNotasPlugin.Properties;
using JaimePlugin;

namespace JaimeCriarNotasPlugin {
    public class JaimeCriarNotasPlugin : IJaimePlugin {
        public string Autor => "Leandro Simões";
        public string Site => @"https://www.facebook.com/Simoes.Leandro.Silva";
        public Image Icone => Resources.CriarNota;
        public string Nome => "Criar notas";
        public string Comando => "notas ";
        public byte Tipo => 0;

        public void Filtrar(string textoEntrada, out string[] retornos, out string[] erros) { throw new NotImplementedException(); }

        public bool Executar(string textoEntrada, out string[] retornos, out string[] erros, out string retornoTextBox, out bool hideForm) {
            retornos = new string[0];
            erros = new string[0];
            retornoTextBox = string.Empty;
            hideForm = true;

            try {
                if (!textoEntrada.StartsWith(Comando)) return false;

                textoEntrada = textoEntrada.Replace(Comando, "");

                if (string.IsNullOrEmpty(textoEntrada)) throw new Exception("Você deve informar o texto para ser adicionado a nota.");

                var diretorio = @"C:\JaimeNotas";
                if (!Directory.Exists(diretorio)) {
                    Directory.CreateDirectory(diretorio);
                }

                var nomeArquivo = "Nota-" + DateTime.Now.ToString("dd-MM-yyy-hh-mm-ss") + ".txt";
                var caminho = diretorio + @"\";
                var arquivoExiste = true;

                while (arquivoExiste) {
                    if (InputBox.Show("Criar nota", "Digite o nome do arquivo", ref nomeArquivo) == DialogResult.OK) {
                        if (string.IsNullOrEmpty(nomeArquivo)) {
                            throw new Exception("Você deve informar o nome do arquivo.");
                        }

                        nomeArquivo += ".txt";
                    } else {
                        throw new Exception("Operação de criar notas cancelada.");
                    }

                    if (!File.Exists(caminho + nomeArquivo)) arquivoExiste = false;

                    if (arquivoExiste) {
                        MessageBox.Show($"Já existe um arquivo chamado \"{nomeArquivo}\", por favor digite outro nome.", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        continue;
                    }

                    File.WriteAllText(caminho + nomeArquivo, textoEntrada);
                }

                if (MessageBox.Show($"Deseja abrir o arquivo \"{nomeArquivo}\" criado?", "Atenção!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK) {
                    Process.Start(caminho + nomeArquivo);
                }

                return true;
            } catch (Exception ex) {
                erros = new[] { ex.Message };
                return false;
            }
        }
    }
}