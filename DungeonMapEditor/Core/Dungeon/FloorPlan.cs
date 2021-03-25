using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel;
using DungeonMapEditor.ViewModel.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class FloorPlan : BaseData<FloorPlan>
    {
        public event EventHandler<NameChangedEventArgs> FloorNameChanged;

        private VeryObservableCollection<RoomAssignment> mRoomAssignments;
        private string mFloorName;
        private string mFloorPlanImageFileName;
        private BitmapImage mFloorPlanImage;

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || RoomAssignments.Any(x => x.RoomPlan.AnyUnsavedChanges);

        public VeryObservableCollection<RoomAssignment> RoomAssignments
        {
            get => mRoomAssignments;
            set
            {
                mRoomAssignments = value;
                InvokePropertyChanged();
            }
        }

        public string FloorName
        {
            get => mFloorName;
            set
            {
                changeManager.ObserveProperty(value);
                mFloorName = value;
                InvokePropertyChanged();
            }
        }

        public string FloorPlanImageFileName
        {
            get => mFloorPlanImageFileName;
            private set
            {
                changeManager.ObserveProperty(value);
                mFloorPlanImageFileName = value;
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public BitmapImage FloorPlanImage
        {
            get => mFloorPlanImage;
            private set
            {
                mFloorPlanImage = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public FloorPlan(string name, string description, double rotation, string guid, List<RoomAssignment> roomAssignments, 
            string floorName, string floorPlanImageFileName) : 
            base(name, description, rotation, guid)
        {
            InitializeLists();
            RoomAssignments.Add(roomAssignments);
            FloorName = floorName;
            if (floorPlanImageFileName != null)
            {
                FloorPlanImageFileName = floorPlanImageFileName;
            }
        }

        /// <summary>
        /// Loads an existing <see cref="FloorPlan"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the floor plan</param>
        public FloorPlan(FileInfo fi) : base(fi)
        {
            InitializeLists();
            Load();
            string floorImageFilePath = Path.Combine(fi.Directory.FullName, "floors", Name + ".png");
            if (File.Exists(floorImageFilePath))
            {
                mFloorPlanImage = Helper.FileToBitmapImage(floorImageFilePath);
            }

            changeManager.ResetObservers();
        }

        /// <summary>
        /// Used to create a new <see cref="FloorPlan"/>
        /// </summary>
        /// <param name="floorName">The name of this floor</param>
        public FloorPlan(string floorName) : base(floorName, "", 0)
        {
            InitializeLists();
            isFile = true;
            FloorName = "";
            FloorName = floorName;
        }

        private void InitializeLists()
        {
            mRoomAssignments = new VeryObservableCollection<RoomAssignment>("RoomAssignments", changeManager, ViewModelMessage.RoomsChanged);
        }

        public override void Delete()
        {
            base.Delete();
            /*string imagePath = Path.Combine(filePath, Name + ".png");
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }*/
        }

        public void Load()
        {
            if (filePath == null)
            {
                return;
            }

            if (!filePath.EndsWith("floors"))
            {
                filePath = Path.Combine(filePath, "floors");
            }
            FloorPlan floorPlan = LoadFile();

            Name = floorPlan?.Name;
            FloorName = floorPlan?.Name;
            RoomAssignments.Clear();
            if (floorPlan?.RoomAssignments != null)
            {
                RoomAssignments.Add(floorPlan.RoomAssignments);
            }
        }

        public void Save(string parentPath = null)
        {
            SaveFloorPlanImage(Path.Combine(parentPath, Name + ".png"));
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if Collection file is being created!");
                }

                fileName = Name + ".json";
                SaveFile(parentPath, this);
            }
            else
            {
                SaveFile(JsonConvert.SerializeObject(this));
            }
        }

        public BitmapImage SaveFloorPlanImage(double inputScaling = 25)
        {
            return SaveFloorPlanImage(Path.Combine(filePath, Name + ".png"), inputScaling);
        }

        public BitmapImage SaveFloorPlanImage(string path, double inputScaling = 25)
        {
            RoomAssignment mostRightRoomAssignment = mRoomAssignments.Count > 0 ? 
                mRoomAssignments.Aggregate((x, y) => AggregateRoomAssignment(x, y, true)) : null;
            RoomAssignment mostBottomRoomAssignment = mRoomAssignments.Count > 0 ?  
                mRoomAssignments.Aggregate((x, y) => AggregateRoomAssignment(x, y, false)) : null;
            double outpuScaling = 50;
            double scaleDiff = outpuScaling / inputScaling;

            double width = mostRightRoomAssignment != null ? 
                (mostRightRoomAssignment.X * scaleDiff) + (mostRightRoomAssignment.RoomPlan.TilesX * outpuScaling) : 1;
            double height = mostBottomRoomAssignment != null ?
                (mostBottomRoomAssignment.Y * scaleDiff) + (mostBottomRoomAssignment.RoomPlan.TilesY * outpuScaling) : 1;

            double roomPlansTopOffset = 50;

            Canvas canvas = new Canvas()
            {
                Width = width,
                Height = (height) + roomPlansTopOffset
            };

            foreach (RoomAssignment roomAssignment in mRoomAssignments)
            {
                RoomControl roomControl = new RoomControl(roomAssignment, false)
                {
                    Width = roomAssignment.RoomPlan.TilesX * outpuScaling,
                    Height = roomAssignment.RoomPlan.TilesY * outpuScaling,
                    BorderThickness = new Thickness(0)
                };
                roomControl.roomImage.Stretch = Stretch.None;

                Canvas.SetLeft(roomControl, (roomAssignment.X + 2) * scaleDiff);
                Canvas.SetTop(roomControl, (roomAssignment.Y + 2) * scaleDiff + roomPlansTopOffset);

                canvas.Children.Add(roomControl);
            }

            DropShadowEffect dropShadowEffect = new DropShadowEffect()
            {
                ShadowDepth = 0,
                BlurRadius = 6
            };

            TextBlock floorNameText = new TextBlock()
            {
                Text = mFloorName,
                TextAlignment = TextAlignment.Center,
                Width = canvas.Width,
                Foreground = Brushes.White,
                FontSize = 46,
                FontWeight = FontWeights.Bold,
                Effect = dropShadowEffect
            };

            TextBlock floorNameTextShadow = new TextBlock()
            {
                Text = floorNameText.Text,
                TextAlignment = floorNameText.TextAlignment,
                Width = floorNameText.Width,
                Foreground = floorNameText.Foreground,
                FontSize = floorNameText.FontSize,
                FontWeight = floorNameText.FontWeight,
                Effect = floorNameText.Effect
            };

            canvas.Children.Add(floorNameTextShadow);
            canvas.Children.Add(floorNameText);

            FloorPlanImage = Helper.ExportToPng_Old(path, canvas);
            FloorPlanImageFileName = Name + ".png";

            return FloorPlanImage;
        }

        private RoomAssignment AggregateRoomAssignment(RoomAssignment x, RoomAssignment y, bool checkWidth, double tileScaling = 25)
        {
            if (checkWidth)
            {
                return x.X + (x.RoomPlan.TilesX * tileScaling) > y.X + (y.RoomPlan.TilesX * tileScaling) ? x : y;
            }
            else
            {
                return x.Y + (x.RoomPlan.TilesY * tileScaling) > y.Y + (y.RoomPlan.TilesY * tileScaling) ? x : y;
            }
        }

        public string GetNotes(string assignmentNotes)
        {
            bool roomsHaveNotes = RoomAssignments.Any(x => x.HasNotes || x.RoomPlan.GetNotes(null) != null);

            if (!roomsHaveNotes)
            {
                return null;
            }

            string notes = "- " + Name + ":";

            if (assignmentNotes != null)
            {
                notes += " (" + assignmentNotes + ")";
            }

            if (roomsHaveNotes)
            {
                notes += "\r\n  - Rooms:";
                foreach (RoomAssignment roomAssignment in RoomAssignments.Where(x => x.HasNotes))
                {
                    string roomNotes = roomAssignment.RoomPlan.GetNotes(roomAssignment.HasNotes ? roomAssignment.Notes : null);
                    if (roomNotes != null)
                    {
                        notes += "\r\n    " + roomNotes;
                    }
                }
            }

            return notes + "\r\n";
        }

        protected virtual void OnFloorNameChanged(NameChangedEventArgs e)
        {
            FloorNameChanged?.Invoke(this, e);
        }
    }
}
