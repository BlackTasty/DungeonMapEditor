using DungeonMapEditor.Core.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public interface IBaseData
    {
        ChangeManager ChangeManager { get; }

        string Guid { get; }

        string Name { get; set; }

        string Description { get; set; }

        double Rotation { get; set; }

        bool UnsavedChanges { get; }

        IBaseData Copy();
    }
}
