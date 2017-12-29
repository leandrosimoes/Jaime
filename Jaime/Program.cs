using System;
using System.Threading;
using System.Windows.Forms;
using Jaime.Extensions;
using Jaime.Helpers;

namespace Jaime {
    static class Program {
        private static string appGuid = "9CF58D7B-2884-42E7-9172-293AEED02896";

        [STAThread]
        static void Main(string[] args) {
            if (args.Length > 0) {
                var path = string.Join(" ", args);
                if (!path.IsNullEmptyOrWhiteSpace()) {
                    var fx = new FavoritosHelper();
                    fx.RegistrarFavorito(path);
                    return;
                }
            }

            using (Mutex mutex = new Mutex(false, appGuid)) {
                if (!mutex.WaitOne(0, false)) {
                    return;
                }
                
                if(JaimeHelper.IsAdministrator())
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Principal());
                } else
                {
                    JaimeHelper.ExecuteAsAdmin(Application.ExecutablePath);
                    return;
                }
            }
        }
    }
}
