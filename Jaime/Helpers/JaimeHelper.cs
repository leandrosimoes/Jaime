using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
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

        public static void ExecuteAsAdmin(string fileName)
        {
            using (var proc = new Process())
            {
                proc.StartInfo.FileName = fileName;
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}