using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.FileSystem
{
    public interface IChangeObserver
    {
        event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        bool UnsavedChanges { get; }

        string PropertyName { get; }

        void Reset();
    }
}
