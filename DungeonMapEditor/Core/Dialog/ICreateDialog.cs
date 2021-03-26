using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dialog
{
    public interface ICreateDialog
    {
        event EventHandler<CreateDialogCompletedEventArgs> DialogCompleted;
    }
}
