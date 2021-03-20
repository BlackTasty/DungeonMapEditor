using DungeonMapEditor.Core.Observer;
using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class Assignment : ViewModelBase
    {
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private string mNotes;
        protected ObserverManager changeManager;

        [JsonIgnore]
        public bool UnsavedChanges => changeManager.UnsavedChanges;

        public string Notes
        {
            get => mNotes;
            set
            {
                changeManager.ObserveProperty(value);
                mNotes = value;
                InvokePropertyChanged();
                InvokePropertyChanged("HasNotes");
            }
        }

        [JsonIgnore]
        public bool HasNotes => !string.IsNullOrWhiteSpace(mNotes);

        [JsonConstructor]
        public Assignment(string notes) : this()
        {
            Notes = notes;
            changeManager.ResetObservers();
        }

        public Assignment()
        {
            changeManager = new ObserverManager();
            changeManager.ChangeObserved += ChangeManager_ChangeObserved;
            Notes = "";
        }

        private void ChangeManager_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
