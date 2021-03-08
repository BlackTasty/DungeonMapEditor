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

        public DialogResult DialogResult => dialogResult;

        public DialogButtonClickedEventArgs(DialogResult dialogResult)
        {
            this.dialogResult = dialogResult;
        }
    }
}
