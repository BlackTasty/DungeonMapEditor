using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Patcher
{
    public class UpdateFoundEventArgs : EventArgs
    {
        private string versionCurrent;
        private string versionNew;

        public string VersionCurrent { get => versionCurrent; }

        public string VersionNew { get => versionNew; }

        public UpdateFoundEventArgs(string versionCurrent, string versionNew)
        {
            this.versionCurrent = versionCurrent;
            this.versionNew = versionNew;
        }
    }
}
