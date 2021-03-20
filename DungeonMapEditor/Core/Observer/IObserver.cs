using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Observer
{
    public interface IObserver
    {
        event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        dynamic GetOriginalValue();

        bool UnsavedChanges { get; }

        string PropertyName { get; }

        void Reset();
    }
}
