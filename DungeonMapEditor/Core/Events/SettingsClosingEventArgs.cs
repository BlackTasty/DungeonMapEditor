using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Events
{
    public class SettingsClosingEventArgs : EventArgs
    {
        private bool unsavedChanges;

        public bool UnsavedChanges => unsavedChanges;

        public SettingsClosingEventArgs(bool unsavedChanges)
        {
            this.unsavedChanges = unsavedChanges;
        }
    }
}
