using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Observer
{
    public class ChangeObservedEventArgs : EventArgs
    {
        private IObserver observer;
        private bool unsavedChanges;
        private dynamic newValue;

        public bool UnsavedChanges => unsavedChanges;

        public dynamic NewValue => newValue;

        public IObserver Observer => observer;

        public ChangeObservedEventArgs(bool unsavedChanges, dynamic newValue, IObserver observer)
        {
            this.unsavedChanges = unsavedChanges;
            this.newValue = newValue;
            this.observer = observer;
        }
    }
}
