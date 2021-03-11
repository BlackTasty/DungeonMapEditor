using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
using DungeonMapEditor.ViewModel.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DungeonMapEditor.Core.Dungeon
{
    public class ProjectFile : JsonFile<ProjectFile>
    {
        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;

        private DateTime mLastModifyDate;
        private string mName;
        private VeryObservableCollection<FloorAssignment> mFloorPlans = new VeryObservableCollection<FloorAssignment>("FloorPlans");
        private VeryObservableCollection<RoomAssignment> mRoomPlans = new VeryObservableCollection<RoomAssignment>("RoomPlans");
        private DocumentSizeType mDocumentSizeType = DocumentSizeType.Image_FullHD;
        private Orientation mDocumentOrientation = Orientation.Vertical;

        public DateTime LastModifyDate => mLastModifyDate;

        public string Name
        {
            get => mName;
            set
            {
                OnProjectNameChanged(new NameChangedEventArgs(mName, value));
                mName = value;
                InvokePropertyChanged();
            }
        }

        public VeryObservableCollection<FloorAssignment> FloorPlans
        {
            get => mFloorPlans;
            set
            {
                mFloorPlans = value;
                InvokePropertyChanged();
            }
        }

        public VeryObservableCollection<RoomAssignment> RoomPlans
        {
            get => mRoomPlans;
            set
            {
                mRoomPlans = value;
                InvokePropertyChanged();
            }
        }

        public DocumentSizeType DocumentSizeType
        {
            get => mDocumentSizeType;
            set
            {
                mDocumentSizeType = value;
                InvokePropertyChanged();
            }
        }

        public Orientation DocumentOrientation
        {
            get => mDocumentOrientation;
            set
            {
                mDocumentOrientation = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public ProjectFile(string name, List<FloorAssignment> floorPlans, List<RoomAssignment> roomPlans, DateTime lastModifyDate,
            DocumentSizeType documentSizeType, Orientation documentOrientation) : this(name)
        {
            mFloorPlans.Add(floorPlans);
            mRoomPlans.Add(roomPlans);
            mLastModifyDate = lastModifyDate;
            mDocumentSizeType = documentSizeType;
            mDocumentOrientation = documentOrientation;
            //InitializeRoomPlans();
        }

        /// <summary>
        /// Used to create a new <see cref="ProjectFile"/>.
        /// </summary>
        /// <param name="name">The name of this project</param>
        public ProjectFile(string name)
        {
            mLastModifyDate = DateTime.Now;
            mName = name;
            fileName = name + ".json";
        }

        /// <summary>
        /// Used to load existing projects from a folder. Folder has to contain a json with the same name as the folder!
        /// </summary>
        /// <param name="di">A <see cref="DirectoryInfo"/> object containing the path to the project folder</param>
        public ProjectFile(DirectoryInfo di) : base(new FileInfo(di.Name + ".json"))
        {
            filePath = Path.Combine(filePath, "Projects", fileName.Substring(0, fileName.LastIndexOf('.')));
            Load();
        }

        public void Save(string parentPath = null)
        {
            mLastModifyDate = DateTime.Now;
            InvokePropertyChanged("LastModifyDate");
            parentPath = parentPath != null ? Path.Combine(parentPath, mName) : filePath; ;
            Directory.CreateDirectory(parentPath);

            foreach (RoomAssignment roomAssignment in RoomPlans)
            {
                roomAssignment.RoomPlan.Save(Path.Combine(parentPath, "rooms"));
            }

            foreach (FloorAssignment floorAssignment in FloorPlans)
            {
                floorAssignment.FloorPlan.Save(Path.Combine(parentPath, "floors"));
            }

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

            Mediator.Instance.NotifyColleagues(ViewModelMessage.RoomsChanged, RoomPlans);
            Mediator.Instance.NotifyColleagues(ViewModelMessage.FloorsChanged, FloorPlans);
            App.LoadHistory();
        }

        public void Load()
        {
            ProjectFile projectFile = LoadFile();

            Name = projectFile.Name;
            FloorPlans.Clear();
            FloorPlans.Add(projectFile.FloorPlans);
            RoomPlans.Clear();
            RoomPlans.Add(projectFile.RoomPlans);
            mLastModifyDate = projectFile.LastModifyDate;
            //InitializeRoomPlans();
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }

        private void InitializeRoomPlans()
        {
            List<RoomAssignment> rooms = new List<RoomAssignment>();
            foreach (FloorAssignment floorAssignment in mFloorPlans)
            {
                if (floorAssignment.FloorPlan != null)
                {
                    rooms.AddRange(floorAssignment.FloorPlan.RoomAssignments);
                }
            }

            mRoomPlans.Add(rooms);
        }
    }
}
