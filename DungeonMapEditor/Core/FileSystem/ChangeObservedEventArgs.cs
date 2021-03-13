using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.FileSystem
{
    public class ChangeObservedEventArgs : EventArgs
    {
        private IChangeObserver observer;
        private bool unsavedChanges;
        private dynamic newValue;

        public bool UnsavedChanges => unsavedChanges;

        public dynamic NewValue => newValue;

        public IChangeObserver Observer => observer;

        public ChangeObservedEventArgs(bool unsavedChanges, dynamic newValue, IChangeObserver observer)
        {
            this.unsavedChanges = unsavedChanges;
            this.newValue = newValue;
            this.observer = observer;
        }
    }
}
