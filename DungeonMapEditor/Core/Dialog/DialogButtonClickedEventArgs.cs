using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dialog
{
    public class DialogButtonClickedEventArgs : EventArgs
    {
        private DialogResult dialogResult;
        private object data;

        public DialogResult DialogResult => dialogResult;

        public object Data => data;

        public DialogButtonClickedEventArgs(DialogResult dialogResult)
        {
            this.dialogResult = dialogResult;
        }

        public DialogButtonClickedEventArgs(DialogResult dialogResult, object data) : this(dialogResult)
        {
            this.data = data;
        }
    }
}
