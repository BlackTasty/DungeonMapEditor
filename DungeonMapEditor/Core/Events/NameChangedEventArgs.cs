using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Events
{
    public class NameChangedEventArgs : EventArgs
    {
        private string oldName;
        private string newName;

        public string OldName => oldName;

        public string NewName => newName;

        public NameChangedEventArgs(string oldName, string newName)
        {
            this.oldName = oldName;
            this.newName = newName;
        }
    }
}
