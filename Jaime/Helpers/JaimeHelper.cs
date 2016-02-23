using System;
using System.Drawing;
using System.Linq;
using Jaime.Extensions;
using Jaime.Libs;
using Microsoft.Win32;

namespace Jaime.Helpers {
    public static class JaimeHelper {
        public static Color ObterCor(string cor) {
            var rgb = cor.Split(',');

            if (rgb.Count() > 1) 
                return Color.FromArgb(255, int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));

            return Color.FromName(cor);
        }
    }
}