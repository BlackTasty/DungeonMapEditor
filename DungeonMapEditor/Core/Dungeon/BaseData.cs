using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public class BaseData : JsonFile, IBaseData
    {
        public event EventHandler<NameChangedEventArgs> NameChanged;

        private string mName;
        private string mDescription;
        private double mRotation;

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged();
            }
        }

        public string Description
        {
            get => mDescription;
            set
            {
                mDescription = value;
                InvokePropertyChanged();
            }
        }

        public double Rotation
        {
            get => mRotation;
            set
            {
                mRotation = value;
                InvokePropertyChanged();
            }
        }

        [JsonConstructor]
        public BaseData(string name, string description, double rotation)
        {
            mName = name;
            mDescription = description;
            mRotation = rotation;
        }

        protected BaseData(FileInfo fi) : base(fi)
        {
        }

        public BaseData() { }

        public BaseData FromJsonFile(FileInfo fi)
        {
            return new BaseData(fi);
        }

        protected virtual void OnNameChanged(NameChangedEventArgs e)
        {
            NameChanged?.Invoke(this, e);
        }
    }
}
