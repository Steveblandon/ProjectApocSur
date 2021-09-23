namespace Projapocsur.Common
{
    using System.IO;
    using Projapocsur.Common.Serialization;
    using UnityEngine;

    public enum StorageMode
    {
        DataPath,
        StreamingAssets,
        Absolute
    }

    /// <summary>
    /// Utility functions for saving and loading data.
    /// </summary>
    public static class StorageUtility
    {
        public static readonly string ClassName = nameof(StorageUtility);

        public static void SaveData(object data, string filename, StorageMode storageMode = StorageMode.DataPath)
        {
            SaveData(data, filename, GetFilePath(storageMode));
        }

        public static void SaveData(object data, string fileName, string filePath)
        {
            if (fileName.Contains(".meta") || !fileName.Contains(".xml"))
            {
                Debug.LogWarning($"{ClassName}: attempted to save non-xml file '{fileName}'. aborted.");
                return;
            }

            string uri = Path.Combine(filePath, fileName);

            using (var stream = File.Open(uri, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                XmlSerializer.Serialize(stream, data);
            }
        }

        public static void LoadData<T>(out T data, string fileName, StorageMode storageMode = StorageMode.DataPath)
            where T : new()
        {
            LoadData(out data, fileName, GetFilePath(storageMode));
        }

        public static void LoadData<T>(out T data, string fileName, string filePath) 
            where T : new()
        {
            data = default(T);

            if (fileName.Contains(".meta"))
            {    
                return;
            }

            if (!fileName.Contains(".xml"))
            {
                Debug.LogWarning($"{ClassName}: attemped to load non-xml file '{fileName}'. aborted.");
                return;
            }

            string uri = Path.Combine(filePath, fileName);

            if (!File.Exists(uri))
            {
                Debug.LogWarning($"{ClassName}: attempted to load '{uri}', but the file does not exist.");
                return;
            }

            using (var stream = File.Open(uri, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDeserializer.Deserialize(stream, out data);
            }
        }

        public static string[] GetDirectoryFiles(string directoryName, StorageMode storageMode = StorageMode.DataPath)
        {
            return GetDirectoryFiles(directoryName, GetFilePath(storageMode));
        }

        public static string[] GetDirectoryFiles(string directoryName, string filePath)
        {
            string uri = Path.Combine(filePath, directoryName);

            if (!Directory.Exists(uri))
            {
                Directory.CreateDirectory(uri);
            }

            return Directory.GetFiles(uri);
        }

        private static string GetFilePath(StorageMode storageMode)
        {
            string filePath = string.Empty;
            switch (storageMode)
            {
                case StorageMode.StreamingAssets:
                    filePath = Application.streamingAssetsPath;
                    break;
                case StorageMode.DataPath:
                    filePath = Path.Combine(Application.dataPath, "Code", "_data");
                    break;
            }

            if (!string.IsNullOrWhiteSpace(filePath) && !Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            return filePath;
        }
    }
}
