namespace Projapocsur.Common.Utilities
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using UnityEngine;

    public enum StorageMode
    {
        StreamingAssets,
        Absolute
    }

    /// <summary>
    /// Utility functions for saving and loading data.
    /// </summary>
    public static class StorageUtility
    {
        public static readonly string ClassName = nameof(StorageUtility);

        public static void SaveData<T>(T data, string filename, StorageMode storageMode = StorageMode.StreamingAssets)
        {
            SaveData(data, filename, GetFilePath(storageMode));
        }

        public static void SaveData<T>(T data, string fileName, string filePath)
        {
            if (fileName.Contains(".meta") || !fileName.Contains(".xml"))
            {
                Debug.LogWarning($"{ClassName}: attemped to save non-xml file '{fileName}'. aborted.");
                return;
            }

            string uri = Path.Combine(filePath, fileName);

            if (File.Exists(uri))
            {
                File.Delete(uri);
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            using (var stream = File.Open(uri, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                var serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(stream, data, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
            }
        }

        public static void LoadData<T>(out T data, string fileName, StorageMode storageMode = StorageMode.StreamingAssets)
        {
            LoadData(out data, fileName, GetFilePath(storageMode));
        }

        public static void LoadData<T>(out T data, string fileName, string filePath)
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
                var serializer = new XmlSerializer(typeof(T));

                data = (T)serializer.Deserialize(stream);
            }
        }

        public static string[] GetDirectoryFiles(string directoryName, StorageMode storageMode = StorageMode.StreamingAssets)
        {
            string uri = Path.Combine(GetFilePath(storageMode), directoryName);

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
            }

            return filePath;
        }
    }
}
