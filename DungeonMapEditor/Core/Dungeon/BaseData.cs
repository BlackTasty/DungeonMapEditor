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
    public class BaseData<T> : JsonFile<T>, IBaseData
    {
        public event EventHandler<NameChangedEventArgs> NameChanged;

        private string mName;
        private string mDescription;
        private double mRotation;
        private string guid;

        public string Guid => guid;

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

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public BaseData(string name, string description, double rotation, string guid)
        {
            mName = name;
            mDescription = description;
            mRotation = rotation;
            this.guid = guid;
        }

        /// <summary>
        /// Used to create a new <see cref="T"/> object.
        /// </summary>
        /// <param name="name">The name of this object</param>
        /// <param name="description">A description for this object</param>
        /// <param name="rotation">The rotation of this object (clock-wise)</param>
        public BaseData(string name, string description, double rotation) : this()
        {
            mName = name;
            mDescription = description;
            mRotation = rotation;
        }

        /// <summary>
        /// Use to load an existing object of type <see cref="T"/>.
        /// </summary>
        /// <param name="fi">The file to load</param>
        protected BaseData(FileInfo fi) : base(fi)
        {
            guid = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Base constructor which generates a unique GUID for this object.
        /// </summary>
        public BaseData() 
        {
            guid = System.Guid.NewGuid().ToString();
        }

        protected virtual void OnNameChanged(NameChangedEventArgs e)
        {
            NameChanged?.Invoke(this, e);
        }
    }
}
