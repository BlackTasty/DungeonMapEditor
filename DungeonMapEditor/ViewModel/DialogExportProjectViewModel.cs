using DungeonMapEditor.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class DialogExportProjectViewModel : ViewModelBase
    {
        private bool mExportAsPdfSelected;
        private bool mExportAsImageSelected;
        private bool mExportNotes;
        private ExportType exportType;

        public bool ExportAsPdfSelected
        {
            get => mExportAsPdfSelected;
            set
            {
                mExportAsPdfSelected = value;
                ExportAsImageSelected = false;
                InvokePropertyChanged();
                if (value)
                {
                    exportType = ExportType.Pdf;
                }
            }
        }

        public bool ExportAsImageSelected
        {
            get => mExportAsImageSelected;
            set
            {
                mExportAsImageSelected = value;
                ExportAsPdfSelected = false;
                InvokePropertyChanged();
                if (value)
                {
                    exportType = ExportType.Image;
                }
            }
        }

        public bool ExportNotes
        {
            get => mExportNotes;
            set
            {
                mExportNotes = value;
                InvokePropertyChanged();
            }
        }

        public bool IsValid => mExportAsImageSelected || mExportAsPdfSelected;

        public ExportType ExportType => exportType;
    }
}
