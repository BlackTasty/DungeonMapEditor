using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.Observer;
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
using System.Windows;
using System.Windows.Controls;

namespace DungeonMapEditor.Core.Dungeon
{
    public class ProjectFile : JsonFile<ProjectFile>
    {
        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;

        private DateTime mLastModifyDate;
        private string mName;
        private VeryObservableCollection<FloorAssignment> mFloorPlans;
        private VeryObservableCollection<RoomAssignment> mRoomPlans;
        private DocumentSizeType mDocumentSizeType = DocumentSizeType.Image_FullHD;
        private Size mDocumentSize = new Size();
        private Orientation mDocumentOrientation = Orientation.Vertical;
        private string guid;

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || FloorPlans.Any(x => x.AnyUnsavedChanges) || RoomPlans.Any(x => x.AnyUnsavedChanges);

        public string Guid => guid;

        public DateTime LastModifyDate => mLastModifyDate;

        public string Name
        {
            get => mName;
            set
            {
                changeManager.ObserveProperty(value);
                //OnProjectNameChanged(new NameChangedEventArgs(mName, value));
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
                changeManager.ObserveProperty(value);
                mDocumentSizeType = value;
                InvokePropertyChanged();
                UpdateDocumentSize();
            }
        }

        public Orientation DocumentOrientation
        {
            get => mDocumentOrientation;
            set
            {
                changeManager.ObserveProperty(value);
                mDocumentOrientation = value;
                InvokePropertyChanged();
                UpdateDocumentSize();
            }
        }

        [JsonIgnore]
        public Size DocumentSize
        {
            get => mDocumentSize;
            set
            {
                mDocumentSize = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public ProjectFile(string name, string guid, List<FloorAssignment> floorPlans, List<RoomAssignment> roomPlans, DateTime lastModifyDate,
            DocumentSizeType documentSizeType, Orientation documentOrientation) : this(name)
        {
            InitializeLists();
            mFloorPlans.Add(floorPlans);
            mRoomPlans.Add(roomPlans);
            mLastModifyDate = lastModifyDate;
            mDocumentSizeType = documentSizeType;
            mDocumentOrientation = documentOrientation;
            this.guid = guid;
            //InitializeRoomPlans();
        }

        /// <summary>
        /// Used to create a new <see cref="ProjectFile"/>.
        /// </summary>
        /// <param name="name">The name of this project</param>
        public ProjectFile(string name)
        {
            InitializeLists();
            mLastModifyDate = DateTime.Now;
            mName = name;
            fileName = name + ".dm";
            guid = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Used to load existing projects from a folder. Folder has to contain a json with the same name as the folder!
        /// </summary>
        /// <param name="di">A <see cref="DirectoryInfo"/> object containing the path to the project folder</param>
        public ProjectFile(DirectoryInfo di) : base(new FileInfo(Path.Combine(di.FullName, di.Name + ".dm")))
        {
            InitializeLists();
            Load();
        }

        private void InitializeLists()
        {
            mFloorPlans = new VeryObservableCollection<FloorAssignment>("FloorPlans", changeManager);
            mRoomPlans = new VeryObservableCollection<RoomAssignment>("RoomPlans", changeManager);
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

        public void SaveOnlyRooms()
        {
            ProjectFile copy = new ProjectFile(new DirectoryInfo(filePath));
            copy.RoomPlans = RoomPlans;

            copy.Save();
        }

        public void SaveOnlyFloors()
        {
            ProjectFile copy = new ProjectFile(new DirectoryInfo(filePath));
            copy.FloorPlans = FloorPlans;

            copy.Save();
        }

        public void Load()
        {
            ProjectFile projectFile = LoadFile();

            Name = projectFile.Name;
            guid = projectFile.guid;

            FloorPlans.Clear();
            FloorPlans.Add(projectFile.FloorPlans);
            RoomPlans.Clear();
            RoomPlans.Add(projectFile.RoomPlans);
            mLastModifyDate = projectFile.LastModifyDate;
            mDocumentOrientation = projectFile.DocumentOrientation;
            mDocumentSizeType = projectFile.mDocumentSizeType;
            UpdateDocumentSize();
            //InitializeRoomPlans();
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }

        private void UpdateDocumentSize()
        {
            Size size = Helper.GetDocumentSize(DocumentSizeType);

            if (DocumentOrientation == Orientation.Vertical)
            {
                size = new Size(size.Height, size.Width);
            }

            DocumentSize = size;
        }

        public string GetNotes()
        {
            bool haveFloorsNotes = FloorPlans.Any(x => x.HasNotes || x.FloorPlan.GetNotes(null) != null);

            if (!haveFloorsNotes)
            {
                return null;
            }

            string notes = "Floors:";
            foreach (FloorAssignment floorAssignment in FloorPlans)
            {
                string floorNotes = floorAssignment.FloorPlan.GetNotes(floorAssignment.HasNotes ? floorAssignment.Notes : null);
                if (floorNotes != null)
                {
                    notes += "\r\n" + floorNotes;
                }
            }

            return notes;
        }

        public override void Delete()
        {
            base.Delete(true);
        }
    }
}
