using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel.Communication
{
    public enum ViewModelMessage
    {
        None,
        RoomsChanged,
        FloorsChanged,
        LoadedCollectionsChanged
    }
}
