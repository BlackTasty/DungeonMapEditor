using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Events
{
    public class CreateDialogCompletedEventArgs : DialogButtonClickedEventArgs
    {
        private object resultObject;

        public object ResultObject => resultObject;

        public CreateDialogCompletedEventArgs(DialogResult dialogResult, object resultObject) : this(dialogResult)
        {
            this.resultObject = resultObject;
        }

        public CreateDialogCompletedEventArgs(DialogResult dialogResult) : base(dialogResult)
        {
        }
    }
}
