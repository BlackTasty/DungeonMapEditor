using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dialog
{
    public interface IDialog
    {
        event EventHandler<DialogButtonClickedEventArgs> DialogCompleted;
    }
}
