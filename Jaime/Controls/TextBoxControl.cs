using System.Windows.Forms;

namespace Jaime.Controls {
    public class TextBoxControl : TextBox {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Control | Keys.Back)) {
                SendKeys.SendWait("^+{LEFT}{BACKSPACE}");
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}