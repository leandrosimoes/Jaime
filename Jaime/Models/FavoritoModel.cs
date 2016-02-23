using System;
using Jaime.Enums;

namespace Jaime.Models {
    public class FavoritoModel {
        public int Id { get; set; }
        public string Caminho { get; set; }
        public string Nome { get; set; }
        public TipoFavorito Tipo { get; set; }
        public bool Principal { get; set; }
        public int QuantidadeVezesAberto { get; set; }
        public string DataUltimaAbertura { get; set; }
    }
}