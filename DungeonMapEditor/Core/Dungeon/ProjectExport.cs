using DungeonMapEditor.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public class ProjectExport
    {
        private ProjectFile projectFile;
        private ExportType exportType;
        private bool exportNotes;

        public ProjectExport(ProjectFile projectFile, ExportType exportType, bool exportNotes)
        {
            this.projectFile = projectFile;
            this.exportType = exportType;
            this.exportNotes = exportNotes;
        }

        public bool ExportProjectTo(string path)
        {
            switch (exportType)
            {
                case ExportType.Image:
                    break;
                case ExportType.Pdf:
                    break;
            }

            if (exportNotes)
            {
                //TODO: Export notes in project
            }

            return true;
        }
    }
}
