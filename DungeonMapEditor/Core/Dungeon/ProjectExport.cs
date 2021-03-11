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
                Background = Brushes.White
            };

            foreach (FloorAssignment floorAssignment in ProjectFile.FloorPlans)
            {
                FloorControl floorControl = new FloorControl(floorAssignment, projectFile)
                {
                    BorderThickness = new Thickness(0)
                };


                Canvas.SetLeft(floorControl, floorControl.FloorAssignment.X);
                Canvas.SetTop(floorControl, floorControl.FloorAssignment.Y);

                canvas.Children.Add(floorControl);
            }

            Helper.ExportToPng(imagePath, canvas);
        }
    }
}
