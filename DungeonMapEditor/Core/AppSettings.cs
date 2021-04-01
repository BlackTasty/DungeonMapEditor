using DungeonMapEditor.Core.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core
{
    public class AppSettings : JsonFile<AppSettings>
    {
        public event EventHandler<PatcherSettingsChangedEventArgs> PatcherSettingsChanged;

        private string projectDirectory = App.DefaultProjectsPath;
        private string collectionDirectory = App.DefaultCollectionPath;
        private double gridScaling = 50;
        private bool updatesEnabled = true;
        private int updateSearchIntervalMin = 15;

        public string ProjectDirectory
        {
            get => projectDirectory;
            set
            {
                changeManager.ObserveProperty(value);
                projectDirectory = value;
            }
        }

        public string CollectionDirectory
        {
            get => collectionDirectory;
            set
            {
                changeManager.ObserveProperty(value);
                collectionDirectory = value;
            }
        }

        public double GridScaling
        {
            get => gridScaling;
            set
            {
                changeManager.ObserveProperty(value);
                gridScaling = value;
            }
        }

        public bool UpdatesEnabled
        {
            get => updatesEnabled;
            set
            {
                changeManager.ObserveProperty(value);
                updatesEnabled = value;
            }
        }

        public int UpdateSearchIntervalMin
        {
            get => updateSearchIntervalMin;
            set
            {
                changeManager.ObserveProperty(value);
                updateSearchIntervalMin = value;
            }
        }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public AppSettings(string projectDirectory, string collectionDirectory, double gridScaling, bool updatesEnabled, int updateSearchIntervalMin)
        {
            ProjectDirectory = projectDirectory;
            CollectionDirectory = collectionDirectory;
            GridScaling = gridScaling;
            UpdatesEnabled = updatesEnabled;
            UpdateSearchIntervalMin = updateSearchIntervalMin;
        }

        /// <summary>
        /// Generate a new <see cref="AppSettings"/> file.
        /// </summary>
        public AppSettings()
        {
            ProjectDirectory = App.DefaultProjectsPath;
            CollectionDirectory = App.DefaultCollectionPath;
            GridScaling = 50;
            UpdatesEnabled = true;
            UpdateSearchIntervalMin = 15;
        }

        /// <summary>
        /// Loads existing <see cref="AppSettings"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the app settings</param>
        public AppSettings(FileInfo fi) : base(fi)
        {
            Load();

            changeManager.ResetObservers();
        }

        public void Load()
        {
            if (filePath == null)
            {
                return;
            }

            AppSettings appSettings = LoadFile();

            ProjectDirectory = appSettings.ProjectDirectory;
            CollectionDirectory = appSettings.CollectionDirectory;
            GridScaling = appSettings.GridScaling;
            UpdatesEnabled = appSettings.UpdatesEnabled;
            UpdateSearchIntervalMin = appSettings.UpdateSearchIntervalMin;
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if AppSettings file is being created!");
                }

                fileName = "settings.json";
                SaveFile(parentPath, this);
            }
            else
            {
                SaveFile(this);
            }
        }

        public void ApplySettings(AppSettings source)
        {
            if (ProjectDirectory != source.ProjectDirectory)
            {
                ProjectDirectory = source.ProjectDirectory;
                App.LoadHistory();
            }
            if (CollectionDirectory != source.CollectionDirectory)
            {
                CollectionDirectory = source.CollectionDirectory;
                App.LoadCollections();
            }
            GridScaling = source.GridScaling;
            UpdateSearchIntervalMin = source.UpdateSearchIntervalMin;
            UpdatesEnabled = source.UpdatesEnabled;
            OnPatcherSettingsChanged(new PatcherSettingsChangedEventArgs(updatesEnabled, updateSearchIntervalMin));
        }

        public void ResetObserver()
        {
            changeManager.ResetObservers();
        }

        protected virtual void OnPatcherSettingsChanged(PatcherSettingsChangedEventArgs e)
        {
            PatcherSettingsChanged?.Invoke(this, e);
        }
    }
}
