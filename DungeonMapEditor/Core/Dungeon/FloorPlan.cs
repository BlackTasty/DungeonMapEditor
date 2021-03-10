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
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class FloorPlan : BaseData<FloorPlan>
    {
        public event EventHandler<NameChangedEventArgs> FloorNameChanged;

        private VeryObservableCollection<RoomAssignment> mRoomAssignments = new VeryObservableCollection<RoomAssignment>("RoomAssignments",
                                                                                ViewModelMessage.RoomsChanged);
        private string mFloorName;
        private string mFloorPlanImageFileName;
        private BitmapImage mFloorPlanImage;

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
                mFloorName = value;
                InvokePropertyChanged();
            }
        }

        public string FloorPlanImageFileName
        {
            get => mFloorPlanImageFileName;
            private set
            {
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
        public FloorPlan(string name, string description, double rotation, string guid, List<RoomAssignment> roomAssignments, string floorName,
            string floorPlanImageFileName) : 
            base(name, description, rotation, guid)
        {
            RoomAssignments.Clear();
            RoomAssignments.Add(roomAssignments);
            mFloorName = floorName;
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
            Load();
        }

        /// <summary>
        /// Used to create a new <see cref="FloorPlan"/>
        /// </summary>
        /// <param name="floorName">The name of this floor</param>
        public FloorPlan(string floorName) : base(floorName, "", 0)
        {
            mFloorName = "";
        }

        public void Load()
        {
            if (!filePath.EndsWith("floors"))
            {
                filePath = Path.Combine(filePath, "floors");
            }
            FloorPlan floorPlan = LoadFile();

            Name = floorPlan.Name;
            FloorName = floorPlan.Name;
            RoomAssignments.Clear();
            RoomAssignments.Add(floorPlan.RoomAssignments);
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

        public BitmapImage SaveFloorPlanImage(string path)
        {
            RoomAssignment mostRightRoomAssignment = mRoomAssignments.Aggregate((x, y) => AggregateRoomAssignment(x, y, true));
            RoomAssignment mostBottomRoomAssignment = mRoomAssignments.Aggregate((x, y) => AggregateRoomAssignment(x, y, false));

            double width = mostRightRoomAssignment.X + (mostRightRoomAssignment.RoomPlan.TilesX * 25);
            double height = mostBottomRoomAssignment.Y + (mostBottomRoomAssignment.RoomPlan.TilesY * 25);

            double roomPlansTopOffset = 68;

            Canvas canvas = new Canvas()
            {
                Width = width,
                Height = height + roomPlansTopOffset
            };

            foreach (RoomAssignment roomAssignment in mRoomAssignments)
            {
                RoomControl roomControl = new RoomControl(roomAssignment)
                {
                    Width = roomAssignment.RoomPlan.TilesX * 25,
                    Height = roomAssignment.RoomPlan.TilesY * 25,
                    BorderThickness = new Thickness(0)
                };

                Canvas.SetLeft(roomControl, roomAssignment.X);
                Canvas.SetTop(roomControl, roomAssignment.Y + roomPlansTopOffset);

                canvas.Children.Add(roomControl);
            }

            TextBlock floorNameText = new TextBlock()
            {
                Text = mFloorName,
                TextAlignment = TextAlignment.Center,
                Width = canvas.Width,
                Foreground = Brushes.White,
                FontSize = 46,
                FontWeight = FontWeights.Bold
            };

            TextBlock floorNameTextShadow = new TextBlock()
            {
                Text = floorNameText.Text,
                TextAlignment = floorNameText.TextAlignment,
                Width = floorNameText.Width,
                Foreground = Brushes.Black,
                FontSize = floorNameText.FontSize,
                FontWeight = floorNameText.FontWeight
            };

            Canvas.SetTop(floorNameTextShadow, 9);
            Canvas.SetLeft(floorNameTextShadow, 1);
            canvas.Children.Add(floorNameTextShadow);

            Canvas.SetTop(floorNameText, 8);
            canvas.Children.Add(floorNameText);

            FloorPlanImage = Helper.ExportToPng(path, canvas);
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

        protected virtual void OnFloorNameChanged(NameChangedEventArgs e)
        {
            FloorNameChanged?.Invoke(this, e);
        }
    }
}
