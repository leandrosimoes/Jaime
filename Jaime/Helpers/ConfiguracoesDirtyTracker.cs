using System;
using System.Windows.Forms;
using Jaime.Enums;
using Jaime.Extensions;

namespace Jaime.Helpers {
    public class ConfiguracoesDirtyTracker : FormDirtyTracker {
        public ConfiguracoesDirtyTracker(Form frm) : base(frm) { }

        public bool PrecisaAtualizarFavoritos { get; set; }

        protected override void FormDirtyTracker_TextChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            ControlesNaoNecessitamAtualizarFavoritos controleNaoAtualiza;
            ControlesNaoNecessitamSalvar controleNaoSalva;

            PrecisaAtualizarFavoritos = control != null && 
                !control.Name.IsNullEmptyOrWhiteSpace() &&
                !Enum.TryParse(control.Name, true, out controleNaoAtualiza);

            if(control != null && !Enum.TryParse(control.Name, true, out controleNaoSalva))
                IsDirty = true;
        }

        protected override void FormDirtyTracker_CheckedChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            ControlesNaoNecessitamAtualizarFavoritos controleNaoAtualiza;
            ControlesNaoNecessitamSalvar controleNaoSalva;

            PrecisaAtualizarFavoritos = control != null &&
                !control.Name.IsNullEmptyOrWhiteSpace() &&
                !Enum.TryParse(control.Name, true, out controleNaoAtualiza);

            if (control != null && !Enum.TryParse(control.Name, true, out controleNaoSalva))
                IsDirty = true;
        }

        protected override void FormDirtyTracker_SelectedIndexChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            ControlesNaoNecessitamAtualizarFavoritos controleNaoAtualiza;
            ControlesNaoNecessitamSalvar controleNaoSalva;

            PrecisaAtualizarFavoritos = control != null &&
                !control.Name.IsNullEmptyOrWhiteSpace() &&
                !Enum.TryParse(control.Name, true, out controleNaoAtualiza);

            if (control != null && !Enum.TryParse(control.Name, true, out controleNaoSalva))
                IsDirty = true;
        }

        protected override void FormDirtyTracker_ValueChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            ControlesNaoNecessitamAtualizarFavoritos controleNaoAtualiza;
            ControlesNaoNecessitamSalvar controleNaoSalva;

            PrecisaAtualizarFavoritos = control != null &&
                !control.Name.IsNullEmptyOrWhiteSpace() &&
                !Enum.TryParse(control.Name, true, out controleNaoAtualiza);

            if (control != null && !Enum.TryParse(control.Name, true, out controleNaoSalva))
                IsDirty = true;
        }

        protected override void FormDirtyTracker_BackColorChanged(object sender, EventArgs e) {
            var control = (Control)sender;
            ControlesNaoNecessitamAtualizarFavoritos controleNaoAtualiza;
            ControlesNaoNecessitamSalvar controleNaoSalva;

            PrecisaAtualizarFavoritos = control != null &&
                !control.Name.IsNullEmptyOrWhiteSpace() &&
                !Enum.TryParse(control.Name, true, out controleNaoAtualiza);

            if (control != null && !Enum.TryParse(control.Name, true, out controleNaoSalva))
                IsDirty = true;
        }

        protected override void AssignHandlersForControlCollection(Control.ControlCollection coll) {
            foreach (Control c in coll) {
                if (c is TextBox) {
                    (c as TextBox).TextChanged
                        += new EventHandler(FormDirtyTracker_TextChanged);

                    (c as TextBox).BackColorChanged
                        += new EventHandler(FormDirtyTracker_BackColorChanged);
                }

                if (c is CheckBox)
                    (c as CheckBox).CheckedChanged
                        += new EventHandler(FormDirtyTracker_CheckedChanged);

                if (c is ComboBox)
                    (c as ComboBox).SelectedIndexChanged
                        += new EventHandler(FormDirtyTracker_SelectedIndexChanged);

                if (c is ListBox)
                    (c as ListBox).SelectedIndexChanged
                        += new EventHandler(FormDirtyTracker_SelectedIndexChanged);

                if (c is NumericUpDown)
                    (c as NumericUpDown).ValueChanged
                        += new EventHandler(FormDirtyTracker_ValueChanged);

                if (c.HasChildren)
                    AssignHandlersForControlCollection(c.Controls);
            }
        }
    }
}