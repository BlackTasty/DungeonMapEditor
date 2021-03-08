using DungeonMapEditor.Core.Dungeon.Assignment;
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
    public class ProjectFile : JsonFile<ProjectFile>
    {
        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;

        private string mName;
        private VeryObservableCollection<FloorAssignment> mFloorPlans = new VeryObservableCollection<FloorAssignment>("FloorPlans");

        public string Name
        {
            get => mName;
            set
            {
                OnProjectNameChanged(new NameChangedEventArgs(mName, value));
                mName = value;
            }
        }

        public VeryObservableCollection<FloorAssignment> FloorPlans
        {
            get => mFloorPlans;
            set
            {
                mFloorPlans = value;
            }
        }

        [JsonConstructor]
        public ProjectFile(string name, List<FloorAssignment> floorPlans)
        {
            mName = name;
            mFloorPlans.Add(floorPlans);
        }

        public ProjectFile(string name)
        {
            mName = name;
        }

        public ProjectFile(DirectoryInfo fi) : base(new FileInfo(fi.Name + ".json"))
        {
            Load();
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if Collection file is being created!");
                }

                SaveFile(parentPath, this);
            }
            else
            {
                SaveFile(JsonConvert.SerializeObject(this));
            }
        }

        public void Load()
        {
            ProjectFile projectFile = LoadFile();

            Name = projectFile.Name;
            FloorPlans.Add(projectFile.FloorPlans);
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }
    }
}
