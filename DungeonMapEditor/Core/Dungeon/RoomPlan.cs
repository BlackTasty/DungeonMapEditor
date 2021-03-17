using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.FileSystem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class RoomPlan : BaseData<RoomPlan>
    {
        public event EventHandler<EventArgs> GridSizeChanged;

        private string assignedProjectName;
        private int mRoomNumber = 1;
        private int mTilesX = 1;
        private int mTilesY = 1;
        private string mRoomPlanImageFileName;
        private BitmapImage mRoomPlanImage;

        private bool disableGridGeneration;

        public List<TileAssignment> TileAssignments { get; set; }

        public List<PlaceableAssignment> PlaceableAssignments { get; set; }

        //public List<FloorChangeTile> FloorChangeTiles { get; set; }

        public int RoomNumber
        {
            get => mRoomNumber;
            set
            {
                changeManager.ObserveProperty(value);
                mRoomNumber = value;
                InvokePropertyChanged();
            }
        }

        /// <summary>
        /// Amount of tiles in X coordinate
        /// </summary>
        public int TilesX
        {
            get => mTilesX;
            set
            {
                changeManager.ObserveProperty(value);
                GenerateTileGrid(mTilesX, value, mTilesY, mTilesY);
                mTilesX = value;
                InvokePropertyChanged();
                OnGridSizeChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Amount of tiles in Y coordinate
        /// </summary>
        public int TilesY
        {
            get => mTilesY;
            set
            {
                changeManager.ObserveProperty(value);
                GenerateTileGrid(mTilesX, mTilesX, mTilesY, value);
                mTilesY = value;
                InvokePropertyChanged();
                OnGridSizeChanged(EventArgs.Empty);
            }
        }

        public string AssignedProjectName => assignedProjectName;

        public string RoomPlanImageFileName
        {
            get => mRoomPlanImageFileName;
            private set
            {
                changeManager.ObserveProperty(value);
                mRoomPlanImageFileName = value;
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public BitmapImage RoomPlanImage
        {
            get => mRoomPlanImage;
            private set
            {
                mRoomPlanImage = value;
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public int RoomNumberOverride { get; set; }

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || TileAssignments.Any(x => x.AnyUnsavedChanges) || PlaceableAssignments.Any(x => x.AnyUnsavedChanges);

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public RoomPlan(string name, string description, double rotation, List<TileAssignment> tileAssignments,
            List<PlaceableAssignment> placeableAssignments/*, List<FloorChangeTile> floorChangeTiles*/, int tilesX, int tilesY,
            string assignedProjectName, string roomPlanImageFileName, int roomNumber) :
            base(name, description, rotation)
        {
            TileAssignments = new List<TileAssignment>();
            disableGridGeneration = true;
            TilesX = tilesX;
            TilesY = tilesY;
            disableGridGeneration = false;
            GenerateTileGrid(1, tilesX, 1, tilesY);
            foreach (TileAssignment tileAssignment in tileAssignments.Where(x => x.TileGuid != null))
            {
                int index = TileAssignments.FindIndex(x => x.X == tileAssignment.X && x.Y == tileAssignment.Y);

                if (index >= 0)
                {
                    TileAssignments[index] = tileAssignment;
                }
            }
            PlaceableAssignments = placeableAssignments;
            //FloorChangeTiles = floorChangeTiles;
            this.assignedProjectName = assignedProjectName;
            RoomPlanImageFileName = roomPlanImageFileName;
            RoomNumber = roomNumber;
        }

        /// <summary>
        /// Used to create a new <see cref="RoomPlan"/>.
        /// </summary>
        /// <param name="name">The name of this room.</param>
        /// <param name="tilesX">The width of this room in tiles (1 tile = 5 ft.)</param>
        /// <param name="tilesY">The height of this room in tiles (1 tile = 5 ft.)</param>
        /// <param name="assignedProject">The project this <see cref="RoomPlan"/> belongs to</param>
        public RoomPlan(string name, int roomNumber, int tilesX, int tilesY, ProjectFile assignedProject)
        {
            isFile = true;
            Name = "";
            Name = name;
            disableGridGeneration = true;
            TilesX = tilesX;
            TilesY = tilesY;
            disableGridGeneration = false;
            assignedProjectName = assignedProject.Name;
            RoomNumber = roomNumber;

            TileAssignments = new List<TileAssignment>();
            PlaceableAssignments = new List<PlaceableAssignment>();
            GenerateTileGrid(1, tilesX, 1, tilesY);
            //FloorChangeTiles = new List<FloorChangeTile>();
        }

        /// <summary>
        /// Loads an existing <see cref="RoomPlan"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the <see cref="RoomPlan"/> file</param>
        public RoomPlan(FileInfo fi) : base(fi)
        {
            Load();

            string roomImageFilePath = Path.Combine(fi.Directory.FullName, "rooms", Name + ".png");
            if (File.Exists(roomImageFilePath))
            {
                mRoomPlanImage = Helper.FileToBitmapImage(roomImageFilePath);
            }

            changeManager.ResetObservers();
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

        public void Save(string parentPath = null)
        {
            SaveRoomPlanImage(Path.Combine(parentPath, Name + ".png"));
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

        public void Load()
        {
            if (filePath == null)
            {
                return;
            }
            if (!filePath.EndsWith("rooms"))
            {
                filePath = Path.Combine(filePath, "rooms");
            }
            RoomPlan roomPlan = LoadFile();

            disableGridGeneration = true;
            TilesX = roomPlan?.TilesX ?? 1;
            TilesY = roomPlan?.TilesY ?? 1;
            disableGridGeneration = false;
            TileAssignments = roomPlan?.TileAssignments;
            foreach (var tileAssignment in TileAssignments)
            {
                tileAssignment.ChangeObserved += Assignment_ChangeObserved1;
            }
            PlaceableAssignments = roomPlan?.PlaceableAssignments;
            foreach (var placeableAssignment in PlaceableAssignments)
            {
                placeableAssignment.ChangeObserved += Assignment_ChangeObserved1;
            }
            //FloorChangeTiles = roomPlan.FloorChangeTiles;
            Name = roomPlan?.Name;
            assignedProjectName = roomPlan?.AssignedProjectName;
            RoomNumber = roomPlan?.RoomNumber ?? 404;
        }

        private void Assignment_ChangeObserved1(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
        }

        public void GenerateTileGrid(int oldX, int newX, int oldY, int newY)
        {
            if (disableGridGeneration || TileAssignments == null)
            {
                return;
            }

            if (oldX <= newX && oldY <= newY)
            {
                for (int x = oldX; x <= newX; x++)
                {
                    for (int y = oldY; y <= newY; y++)
                    {
                        TileAssignment tileAssignment = new TileAssignment(new Tile(false), x, y);
                        tileAssignment.ChangeObserved += TileAssignment_ChangeObserved;
                        TileAssignments.Add(new TileAssignment(new Tile(false), x, y));
                    }
                }
            }
            else if (oldX > newX || oldY > newY)
            {
                int diffX = oldX - newX;
                int diffY = oldY - newY;
                for (int x = oldX; x >= newX; x--)
                {
                    var target = TileAssignments.FirstOrDefault(t => t.X == x - 1);
                    if (target != null)
                    {
                        if (target.Control.Parent is Canvas grid)
                        {
                            grid.Children.Remove(target.Control);
                        }
                        TileAssignments.Remove(target);
                    }
                }

                for (int y = oldY; y >= newY; y--)
                {
                    var target = TileAssignments.FirstOrDefault(t => t.Y == y - 1);
                    if (target != null)
                    {
                        TileAssignments.Remove(target);
                    }
                }
            }
        }

        private void TileAssignment_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(new ChangeObservedEventArgs(AnyUnsavedChanges, e.NewValue, e.Observer));
        }

        public BitmapImage SaveRoomPlanImage(string path)
        {
            Canvas canvas = new Canvas()
            {
                Width = TilesX * 50,
                Height = TilesY * 50
            };

            foreach (TileAssignment tileAssignment in TileAssignments)
            {
                TileControl tileControl = new TileControl(tileAssignment, false)
                {
                    Width = 50,
                    Height = 50,
                    BorderThickness = new Thickness(0)
                };


                Canvas.SetLeft(tileControl, tileAssignment.CanvasX);
                Canvas.SetTop(tileControl, tileAssignment.CanvasY);

                canvas.Children.Add(tileControl);
            }
            
            foreach (PlaceableAssignment placeableAssignment in PlaceableAssignments)
            {
                PlaceableControl placeableControl = new PlaceableControl(placeableAssignment, new Size(), false)
                {
                    Width = placeableAssignment.Width,
                    Height = placeableAssignment.Height,
                    BorderThickness = new Thickness(0)
                };

                Canvas.SetLeft(placeableControl, placeableAssignment.PositionX);
                Canvas.SetTop(placeableControl, placeableAssignment.PositionY);
                canvas.Children.Add(placeableControl);
            }

            DropShadowEffect dropShadowEffect = new DropShadowEffect()
            {
                ShadowDepth = 0,
                BlurRadius = 6
            };

            TextBlock roomNumberText = new TextBlock()
            {
                Text = RoomNumberOverride > 0 ? RoomNumberOverride.ToString() : mRoomNumber.ToString(),
                TextAlignment = TextAlignment.Center,
                Width = canvas.Width,
                Foreground = Brushes.White,
                FontSize = 42,
                FontWeight = FontWeights.Bold,
                Effect = dropShadowEffect
            };

            TextBlock roomNumberTextShadow = new TextBlock()
            {
                Text = roomNumberText.Text,
                TextAlignment = roomNumberText.TextAlignment,
                Width = roomNumberText.Width,
                Foreground = roomNumberText.Foreground,
                FontSize = roomNumberText.FontSize,
                FontWeight = roomNumberText.FontWeight,
                Effect = roomNumberText.Effect
            };
            double topPos = canvas.Height / 2 - 30;

            Canvas.SetTop(roomNumberTextShadow, topPos);
            canvas.Children.Add(roomNumberTextShadow);

            Canvas.SetTop(roomNumberText, topPos);
            canvas.Children.Add(roomNumberText);

            RoomPlanImage = Helper.ExportToPng(path, canvas);
            RoomPlanImageFileName = Name + ".png";

            return RoomPlanImage;
        }

        public string GetNotes(string assignmentNotes)
        {
            bool tilesHaveNotes = TileAssignments.Any(x => x.HasNotes);
            bool placeablesHaveNotes = PlaceableAssignments.Any(x => x.HasNotes);

            if (!tilesHaveNotes && !placeablesHaveNotes)
            {
                return null;
            }

            string notes = "- " + Name + ":";

            if (assignmentNotes != null)
            {
                notes += " (" + assignmentNotes + ")";
            }

            if (tilesHaveNotes)
            {
                notes += "\r\n      - Tiles:";
                foreach (TileAssignment tileAssignment in TileAssignments.Where(x => x.HasNotes))
                {
                    notes += "\r\n        - " + tileAssignment.Notes;
                }
            }

            if (placeablesHaveNotes)
            {
                notes += "\r\n      - Objects:";
                foreach (PlaceableAssignment placeableAssignment in PlaceableAssignments.Where(x => x.HasNotes))
                {
                    notes += "\r\n        - " + placeableAssignment.Notes;
                }
            }

            return notes + "\r\n";
        }

        protected virtual void OnGridSizeChanged(EventArgs e)
        {
            GridSizeChanged?.Invoke(this, e);
        }
    }
}
