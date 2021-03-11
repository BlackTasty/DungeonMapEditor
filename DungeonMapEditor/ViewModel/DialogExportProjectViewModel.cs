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
                if (value)
                {
                    ExportAsImageSelected = false;
                    exportType = ExportType.Pdf;
                }
                mExportAsPdfSelected = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
            }
        }

        public bool ExportAsImageSelected
        {
            get => mExportAsImageSelected;
            set
            {
                if (value)
                {
                    ExportAsPdfSelected = false;
                    exportType = ExportType.Image;
                }
                mExportAsImageSelected = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsValid");
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
