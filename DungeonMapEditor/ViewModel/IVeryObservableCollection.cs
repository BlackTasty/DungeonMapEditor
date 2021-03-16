using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    public interface IVeryObservableCollection
    {
        bool UnsavedChanged { get; }

        bool AnyUnsavedChanges { get; }
    }
}
