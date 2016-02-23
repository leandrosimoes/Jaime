using System;
using System.Windows.Forms;

namespace Jaime.Helpers {
    public abstract class FormDirtyTracker {
        private Form _frmTraked;
        private bool _isDirty;

        protected FormDirtyTracker(Form frm) {
            _frmTraked = frm;
            AssignHandlersForControlCollection(frm.Controls);
        }

        public bool IsDirty {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public void SetAsDirty() {
            _isDirty = true;
        }

        public void SetAsClean() {
            _isDirty = false;
        }

        protected virtual void FormDirtyTracker_TextChanged(object sender, EventArgs e) {
            IsDirty = true;
        }

        protected virtual void FormDirtyTracker_CheckedChanged(object sender, EventArgs e) {
            IsDirty = true;
        }

        protected virtual void FormDirtyTracker_SelectedIndexChanged(object sender, EventArgs e) {
            IsDirty = true;
        }

        protected virtual void FormDirtyTracker_ValueChanged(object sender, EventArgs e) {
            IsDirty = true;
        }

        protected virtual void FormDirtyTracker_BackColorChanged(object sender, EventArgs e) {
            IsDirty = true;
        }

        protected virtual void AssignHandlersForControlCollection(Control.ControlCollection coll) {
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