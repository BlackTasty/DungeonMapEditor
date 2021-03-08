using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class CreateProjectViewModel : ViewModelBase
    {
        private string mProjectName = "Dangerous dungeon of danger";

        public string ProjectName
        {
            get => mProjectName;
            set
            {
                mProjectName = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(mProjectName);
    }
}
