﻿using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon.Assignment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class RoomPlan : BaseData<RoomPlan>
    {
        public event EventHandler<EventArgs> GridSizeChanged;

        private string assignedProjectName;
        private int mTilesX = 1;
        private int mTilesY = 1;
        private string mRoomPlanImagePath;
        private BitmapImage mRoomPlanImage;

        public List<TileAssignment> TileAssignments { get; set; }

        public List<PlaceableAssignment> PlaceableAssignments { get; set; }

        //public List<FloorChangeTile> FloorChangeTiles { get; set; }

        /// <summary>
        /// Amount of tiles in X coordinate
        /// </summary>
        public int TilesX
        {
            get => mTilesX;
            set
            {
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
                GenerateTileGrid(mTilesX, mTilesX, mTilesY, value);
                mTilesY = value;
                InvokePropertyChanged();
                OnGridSizeChanged(EventArgs.Empty);
            }
        }

        public string AssignedProjectName => assignedProjectName;

        public string RoomPlanImagePath
        {
            get => mRoomPlanImagePath;
            private set
            {
                mRoomPlanImagePath = value;
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

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public RoomPlan(string name, string description, double rotation, List<TileAssignment> tileAssignments,
            List<PlaceableAssignment> placeableAssignments/*, List<FloorChangeTile> floorChangeTiles*/, int tilesX, int tilesY,
            string assignedProjectName, string roomPlanImagePath) :
            base(name, description, rotation)
        {
            TileAssignments = new List<TileAssignment>();
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
            mTilesX = tilesX;
            mTilesY = tilesY;
            this.assignedProjectName = assignedProjectName;
            mRoomPlanImagePath = roomPlanImagePath;
            if (roomPlanImagePath != null)
            {
                mRoomPlanImage = Helper.FileToBitmapImage(roomPlanImagePath);
            }
        }

        /// <summary>
        /// Used to create a new <see cref="RoomPlan"/>.
        /// </summary>
        /// <param name="name">The name of this room.</param>
        /// <param name="tilesX">The width of this room in tiles (1 tile = 5 ft.)</param>
        /// <param name="tilesY">The height of this room in tiles (1 tile = 5 ft.)</param>
        /// <param name="assignedProject">The project this <see cref="RoomPlan"/> belongs to</param>
        public RoomPlan(string name, int tilesX, int tilesY, ProjectFile assignedProject)
        {
            Name = name;
            mTilesX = tilesX;
            mTilesY = tilesY;
            assignedProjectName = assignedProject.Name;

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
        }

        public void Save(string parentPath = null)
        {
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

            SaveRoomPlanImage(Path.Combine(parentPath, Name + ".png"));
        }

        public void Load()
        {
            filePath = Path.Combine(filePath, "rooms");
            RoomPlan roomPlan = LoadFile();

            TileAssignments = roomPlan.TileAssignments;
            PlaceableAssignments = roomPlan.PlaceableAssignments;
            //FloorChangeTiles = roomPlan.FloorChangeTiles;
            mTilesX = roomPlan.TilesX;
            mTilesY = roomPlan.TilesY;
            Name = roomPlan.Name;
            assignedProjectName = roomPlan.AssignedProjectName;
        }

        public void GenerateTileGrid(int oldX, int newX, int oldY, int newY)
        {
            if (oldX <= newX && oldY <= newY)
            {
                for (int x = oldX; x <= newX; x++)
                {
                    for (int y = oldY; y <= newY; y++)
                    {
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

        private void SaveRoomPlanImage(string path)
        {
            Canvas canvas = new Canvas()
            {
                Width = TilesX * 50,
                Height = TilesY * 50
            };

            foreach (TileAssignment tileAssignment in TileAssignments)
            {
                TileControl tileControl = new TileControl()
                {
                    Width = 50,
                    Height = 50,
                    Tile = tileAssignment.Tile,
                    BorderThickness = new System.Windows.Thickness(0)
                };

                Canvas.SetLeft(tileControl, tileAssignment.CanvasX);
                Canvas.SetTop(tileControl, tileAssignment.CanvasY);

                canvas.Children.Add(tileControl);
            }
            
            foreach (PlaceableAssignment placeableAssignment in PlaceableAssignments)
            {
                PlaceableControl placeableControl = new PlaceableControl(placeableAssignment)
                {
                    Width = placeableAssignment.Width,
                    Height = placeableAssignment.Height,
                    BorderThickness = new System.Windows.Thickness(0)
                };

                Canvas.SetLeft(placeableControl, placeableAssignment.PositionX);
                Canvas.SetTop(placeableControl, placeableAssignment.PositionY);
                canvas.Children.Add(placeableControl);
            }

            RoomPlanImage = Helper.ExportToPng(path, canvas);
            RoomPlanImagePath = RoomPlanImage != null ? path : null;
        }

        protected virtual void OnGridSizeChanged(EventArgs e)
        {
            GridSizeChanged?.Invoke(this, e);
        }
    }
}
