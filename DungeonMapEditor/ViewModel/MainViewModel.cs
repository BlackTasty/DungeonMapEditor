using DungeonMapEditor.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonMapEditor.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private bool mShowDialog;
        private FrameworkElement mDialog;

        public MainViewModel()
        {
            if (!App.IsDesignMode)
            {
                mDialog = new DialogCreateProject();
            }
        }

        public bool ShowDialog
        {
            get => mShowDialog;
            set
            {
                mShowDialog = value;
                InvokePropertyChanged();
                if (!value)
                {
                    Dialog = null;
                }
            }
        }

        public FrameworkElement Dialog
        {
            get => mDialog;
            set
            {
                mDialog = value;
                InvokePropertyChanged();
                if (value != null)
                {
                    ShowDialog = true;
                }
            }
        }
    }
}
