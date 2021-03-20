using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Patcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonMapEditor.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        private bool mShowDialog;
        private FrameworkElement mDialog;
        private UpdateManager updateManager;

        private string mUpdateText = "Idle";
        private bool mIsSearching;
        private bool mIsDownloading;
        private double mDownloadSize;
        private double mDownloadCurrent;
        private bool mIsUpdateReady;

        public UpdateManager UpdateManager => updateManager;

        public UpdateStatus UpdateStatus
        {
            get => updateManager?.Status ?? UpdateStatus.IDLE;
            set => updateManager.Status = value;
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

        public string UpdateText
        {
            get => mUpdateText;
            set
            {
                mUpdateText = value;
                InvokePropertyChanged();
            }
        }

        public bool IsSearching
        {
            get
            {
                if (!mIsSearching)
                {
                    return mIsUpdateReady;
                }
                else
                {
                    return mIsSearching;
                }
            }
            set
            {
                mIsSearching = value;
                if (!value && !mIsUpdateReady)
                {
                    new Thread(o => {
                        Thread.Sleep(3000);
                        InvokePropertyChanged();
                    }).Start();
                }
                else
                {
                    InvokePropertyChanged();
                }
            }
        }

        public bool IsDownloading
        {
            get => mIsDownloading;
            set
            {
                mIsDownloading = value;
                InvokePropertyChanged();
            }
        }

        public bool IsUpdateReady
        {
            get => mIsUpdateReady;
            set
            {
                mIsUpdateReady = value;
                InvokePropertyChanged();
            }
        }

        public double DownloadSize
        {
            get => mDownloadSize;
            set
            {
                mDownloadSize = value;
                InvokePropertyChanged();
            }
        }

        public double DownloadCurrent
        {
            get => mDownloadCurrent;
            set
            {
                mDownloadCurrent = value;
                InvokePropertyChanged();
            }
        }

        public MainViewModel()
        {
            updateManager = new UpdateManager();
            updateManager.DownloadProgressChanged += UpdateManager_DownloadProgressChanged;
            updateManager.UpdateFailed += UpdateManager_UpdateFailed;
            updateManager.SearchStatusChanged += UpdateManager_SearchStatusChanged;
            updateManager.StatusChanged += UpdateManager_StatusChanged;

            if (!App.IsDesignMode)
            {
                mDialog = new DialogCreateProject();
                updateManager.CheckForUpdates();
            }
        }

        private void UpdateManager_StatusChanged(object sender, UpdateStatus e)
        {
            InvokePropertyChanged("UpdateStatus");

            switch (e)
            {
                case UpdateStatus.IDLE:
                    UpdateText = "Idle";
                    break;
                case UpdateStatus.SEARCHING:
                    IsSearching = true;
                    UpdateText = "Searching for updates...";
                    break;
                case UpdateStatus.UPDATES_FOUND:
                    UpdateText = "Updates found!";
                    IsUpdateReady = true;
                    break;
                case UpdateStatus.DOWNLOADING:
                    IsUpdateReady = false;
                    IsDownloading = true;
                    UpdateText = "Downloading update...";
                    break;
                case UpdateStatus.EXTRACTING:
                case UpdateStatus.INSTALLING:
                    IsDownloading = false;
                    UpdateText = "Extracting files...";
                    break;
                case UpdateStatus.READY:
                    UpdateText = "Update installed!";
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "DungeonMapEditor.exe");
                    Application.Current.MainWindow.Close();
                    break;
                case UpdateStatus.UPTODATE:
                    UpdateText = "Everything is up-to-date!";
                    break;
            }
        }

        private void UpdateManager_SearchStatusChanged(object sender, bool e)
        {
            IsSearching = e;
        }

        private void UpdateManager_UpdateFailed(object sender, UpdateFailedEventArgs e)
        {
            UpdateText = e.ErrorMessage;
        }

        private void UpdateManager_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            UpdateText = "Downloading - " +
                Math.Round((e.BytesReceived / 1024d) / 1024d, 2).ToString("0.00") + " from " +
                Math.Round((e.TotalBytesToReceive / 1024d) / 1024d, 2).ToString("0.00") +
                " (" + updateManager.CalculateSpeed(e.BytesReceived) + ")";
            DownloadSize = e.TotalBytesToReceive;
            DownloadCurrent = e.BytesReceived;
        }
    }
}
