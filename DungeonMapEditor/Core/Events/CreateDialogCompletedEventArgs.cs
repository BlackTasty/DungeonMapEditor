using DungeonMapEditor.Core.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Events
{
    public class CreateDialogCompletedEventArgs<T> : EventArgs
    {
        private T resultObject;
        private DialogResult dialogResult;

        public T ResultObject => resultObject;

        public DialogResult DialogResult => dialogResult;

        public CreateDialogCompletedEventArgs(DialogResult dialogResult, T resultObject) : this(dialogResult)
        {
            this.resultObject = resultObject;
        }

        public CreateDialogCompletedEventArgs(DialogResult dialogResult)
        {
            this.dialogResult = dialogResult;
        }
    }
}
