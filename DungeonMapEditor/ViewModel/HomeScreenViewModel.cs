using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class HomeScreenViewModel : ViewModelBase
    {
        private VeryObservableStackCollection<ProjectFile> mProjectHistory = new VeryObservableStackCollection<ProjectFile>("ProjectHistory", 7);

        public VeryObservableStackCollection<ProjectFile> ProjectHistory
        {
            get => mProjectHistory;
            set
            {
                mProjectHistory = value;
                InvokePropertyChanged();
            }
        }

        public bool HistoryIsEmpty => ProjectHistory.Count == 0;

        public HomeScreenViewModel()
        {
            mProjectHistory.TriggerAlso("HistoryIsEmpty");
        }
    }
}
