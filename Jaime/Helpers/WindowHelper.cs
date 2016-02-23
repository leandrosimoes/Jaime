using System;
using System.Runtime.InteropServices;

namespace Jaime.Helpers {
    public static class WindowHelper {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("user32.dll")]
        static extern bool BringWindowToTop(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        private const int SW_SHOW = 5;
        private const int SW_RESTORE = 9;

        private const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
        private const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
        private const int SPIF_SENDCHANGE = 0x2;

        //Esta função utiliza a API do Windows para forçar o foco no form quando ele é mostrado na tela.
        public static void Activate(IntPtr hWnd) {
            if (IsIconic(hWnd))
                ShowWindowAsync(hWnd, SW_RESTORE);

            ShowWindowAsync(hWnd, SW_SHOW);

            SetForegroundWindow(hWnd);

            IntPtr foregroundWindow = GetForegroundWindow();
            IntPtr Dummy = IntPtr.Zero;

            uint foregroundThreadId = GetWindowThreadProcessId(foregroundWindow, Dummy);
            uint thisThreadId = GetWindowThreadProcessId(hWnd, Dummy);

            if (AttachThreadInput(thisThreadId, foregroundThreadId, true)) {
                BringWindowToTop(hWnd);
                SetForegroundWindow(hWnd);
                AttachThreadInput(thisThreadId, foregroundThreadId, false);
            }

            if (GetForegroundWindow() != hWnd) {
                IntPtr Timeout = IntPtr.Zero;
                SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, Timeout, 0);
                SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, Dummy, SPIF_SENDCHANGE);
                BringWindowToTop(hWnd);
                SetForegroundWindow(hWnd);
                SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, Timeout, SPIF_SENDCHANGE);
            }
        }
    }
}