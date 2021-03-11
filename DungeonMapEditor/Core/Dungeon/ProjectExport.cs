using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DungeonMapEditor.Core.Dungeon
{
    public class ProjectExport
    {
        private ProjectFile projectFile;
        private ExportType exportType;
        private bool exportNotes;

        public ProjectFile ProjectFile => projectFile;

        public ProjectExport(ProjectFile projectFile, ExportType exportType, bool exportNotes)
        {
            this.projectFile = projectFile;
            this.exportType = exportType;
            this.exportNotes = exportNotes;
        }

        public string ExportProject()
        {
            return ExportProjectTo(Path.Combine(projectFile.FilePath, "exported"));
        }

        public string ExportProjectTo(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string dungeonMapFilePath = null;
            switch (exportType)
            {
                case ExportType.Image:
                    dungeonMapFilePath = Path.Combine(path, projectFile.Name + ".png");
                    SavePlanAsImage(dungeonMapFilePath);
                    break;
                case ExportType.Pdf:
                    break;
            }

            if (exportNotes)
            {
                //TODO: Export notes in project
                string projectNotes = projectFile.GetNotes();

                if (projectNotes != null)
                {
                    File.WriteAllText(Path.Combine(path, "notes.txt"), projectFile.GetNotes());
                }
            }

            return dungeonMapFilePath;
        }

        private void SavePlanAsImage(string imagePath)
        {
            Canvas canvas = new Canvas()
            {
                Width = projectFile.DocumentSize.Width,
                Height = projectFile.DocumentSize.Height,
                Background = new DrawingBrush()
                {
                    TileMode = TileMode.Tile,
                    Viewport = new Rect(-0.5, -0.5, 25, 25),
                    Viewbox = new Rect(0, 0, 49, 49),
                    ViewboxUnits = BrushMappingMode.Absolute,
                    ViewportUnits = BrushMappingMode.Absolute,
                    Drawing = new GeometryDrawing()
                    {
                        Geometry = new RectangleGeometry(new Rect(0, 0, 50, 50)),
                        Pen = new Pen(Brushes.Black, 2)
                        {
                            DashStyle = new DashStyle(new List<double>() { 2, 4 }, 0)
                        }
                    }
                }
            };

            foreach (FloorAssignment floorAssignment in ProjectFile.FloorPlans)
            {
                FloorControl floorControl = new FloorControl(floorAssignment, projectFile, 0, false)
                {
                    BorderThickness = new Thickness(0)
                };


                Canvas.SetLeft(floorControl, floorControl.FloorAssignment.X + 2);
                Canvas.SetTop(floorControl, floorControl.FloorAssignment.Y + 2);

                canvas.Children.Add(floorControl);
            }

            TextBlock dungeonTitle = new TextBlock()
            {
                Text = projectFile.Name,
                FontSize = 64,
                Padding = new Thickness(8),
                Width = canvas.Width,
                TextAlignment = TextAlignment.Center,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/DungeonMapEditor;component/Resources/Fonts/"), "./#Ace Records")
            };

            canvas.Children.Add(dungeonTitle);

            TextBlock footNote = new TextBlock()
            {
                Text = "1 tile = 5 ft.",
                FontSize = 32,
                Background = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4)
            };

            Canvas.SetBottom(footNote, 0);
            canvas.Children.Add(footNote);

            Helper.ExportToPng(imagePath, canvas, Brushes.White);
        }
    }
}
