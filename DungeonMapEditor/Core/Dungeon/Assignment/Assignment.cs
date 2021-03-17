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
        private string mNotes;

        public string Notes
        {
            get => mNotes;
            set
            {
                mNotes = value;
                InvokePropertyChanged();
                InvokePropertyChanged("HasNotes");
            }
        }

        [JsonIgnore]
        public bool HasNotes => !string.IsNullOrWhiteSpace(mNotes);

        [JsonConstructor]
        public Assignment(string notes)
        {
            Notes = notes;
        }

        public Assignment() { }
    }
}
