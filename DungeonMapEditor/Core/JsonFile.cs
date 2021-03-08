using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core
{
    public class JsonFile<T> : ViewModelBase
    {
        protected string filePath;
        protected string fileName;
        protected bool fromFile;
        private DateTime lastModifyDate;

        /// <summary>
        /// Path excluding file name
        /// </summary>
        [JsonIgnore]
        public string FilePath => filePath;

        [JsonIgnore]
        public string FileName => fileName;

        [JsonIgnore]
        public bool FromFile => fromFile;

        [JsonIgnore]
        public DateTime LastModifyDate => lastModifyDate;

        public JsonFile() { }

        public JsonFile(FileInfo fi)
        {
            filePath = fi.DirectoryName;
            fileName = fi.Name;
            lastModifyDate = fi.LastWriteTime;

            fromFile = true;
        }

        public JsonFile(DirectoryInfo di)
        {
            filePath = di.Parent.FullName;
            fileName = di.Name;
            lastModifyDate = di.LastWriteTime;

            fromFile = true;
        }

        public string GetFullPath()
        {
            return Path.Combine(filePath, fileName);
        }

        protected void SaveFile(string json)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new Exception("FilePath must be set! Use SaveFile(string filePath, string json) to create a new file instead!");
            }

            File.WriteAllText(Path.Combine(filePath, fileName), json);
            fromFile = true;
        }

        protected void SaveFile(string filePath, T @object)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            this.filePath = filePath;
            SaveFile(JsonConvert.SerializeObject(@object));
        }

        protected T LoadFile()
        {
            if (!fromFile)
            {
                return default;
            }

            string json = File.ReadAllText(Path.Combine(filePath, fileName));
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
