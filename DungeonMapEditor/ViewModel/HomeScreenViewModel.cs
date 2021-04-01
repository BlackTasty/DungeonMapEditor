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
        public VeryObservableStackCollection<ProjectFile> ProjectHistory => App.ProjectHistory;

        public bool IsHistoryEmpty => App.IsHistoryEmpty;

        public HomeScreenViewModel()
        {
            App.ProjectsChanged += App_ProjectsChanged;
        }

        private void App_ProjectsChanged(object sender, EventArgs e)
        {
            InvokePropertyChanged("IsHistoryEmpty");
        }
    }
}
