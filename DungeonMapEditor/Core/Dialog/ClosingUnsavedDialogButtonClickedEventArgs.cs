using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DungeonMapEditor.Core.Dialog
{
    public class ClosingUnsavedDialogButtonClickedEventArgs : DialogButtonClickedEventArgs, IClosingUnsavedDialogButtonClickedEventArgs
    {
        private dynamic target;
        private TabItem targetTab;

        public dynamic Target => target;

        public TabItem TargetTab => targetTab;

        public ClosingUnsavedDialogButtonClickedEventArgs(dynamic target, TabItem targetTab, DialogResult dialogResult) : this(dialogResult)
        {
            this.target = target;
            this.targetTab = targetTab;
        }

        public ClosingUnsavedDialogButtonClickedEventArgs(DialogResult dialogResult) : base(dialogResult) { }
    }
}
