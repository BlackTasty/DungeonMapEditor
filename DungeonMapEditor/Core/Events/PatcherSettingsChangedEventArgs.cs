using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Events
{
    public class PatcherSettingsChangedEventArgs : EventArgs
    {
        private bool updatesEnabled;
        private int updateInterval;

        public bool UpdatesEnabled => updatesEnabled;

        public int UpdateInterval => updateInterval;

        public PatcherSettingsChangedEventArgs(bool updatesEnabled, int updateInterval)
        {
            this.updateInterval = updateInterval;
            this.updatesEnabled = updatesEnabled;
        }
    }
}
