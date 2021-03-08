using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonMapEditor.Core.Events
{
    public class OpenDialogEventArgs : EventArgs
    {
        private FrameworkElement dialog;

        public FrameworkElement Dialog => dialog;

        public OpenDialogEventArgs(FrameworkElement dialog)
        {
            this.dialog = dialog;
        }
    }
}
