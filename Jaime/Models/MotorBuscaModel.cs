using System;
using Jaime.Enums;

namespace Jaime.Models {
    public class MotorBuscaModel {
        public int Id { get; set; }
        public string Comando { get; set; }
        public string Url { get; set; }
        public string Parametros { get; set; }
    }
}