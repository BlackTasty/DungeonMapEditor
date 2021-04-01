using DungeonMapEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        private AppSettings mTempSettings;

        public SettingsViewModel()
        {
            if (!App.IsDesignMode)
            {
                mTempSettings = new AppSettings(App.Settings.ProjectDirectory,
                                                App.Settings.CollectionDirectory,
                                                App.Settings.GridScaling,
                                                App.Settings.UpdatesEnabled,
                                                App.Settings.UpdateSearchIntervalMin);
            }
        }

        public AppSettings TempSettings
        {
            get => mTempSettings;
            set
            {
                mTempSettings = value;
                InvokePropertyChanged();
            }
        }

        public void ForceUpdate()
        {
            InvokePropertyChanged("TempSettings");
        }
    }
}
