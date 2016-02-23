using System.Windows.Forms;

namespace Jaime.Helpers {
    public static class MessageBoxHelper {
        public static DialogResult Error(string message, MessageBoxButtons buttons = MessageBoxButtons.OK) {
            return MessageBox.Show(message, "Erro!", buttons, MessageBoxIcon.Error);
        }

        public static DialogResult Alert(string message, MessageBoxButtons buttons = MessageBoxButtons.YesNo) {
            return MessageBox.Show(message, "Atenção!", buttons, MessageBoxIcon.Warning);
        }

        public static DialogResult Info(string message, MessageBoxButtons buttons = MessageBoxButtons.OK) {
            return MessageBox.Show(message, "Atenção!", buttons, MessageBoxIcon.Information);
        }

        public static DialogResult Question(string message, MessageBoxButtons buttons = MessageBoxButtons.YesNo) {
            return MessageBox.Show(message, "Atenção!", buttons, MessageBoxIcon.Question);
        }
    }
}