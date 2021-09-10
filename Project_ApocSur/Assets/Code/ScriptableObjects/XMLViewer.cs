namespace Projapocsur.ScriptableObjects
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using Projapocsur.CustomAttributes;
    using UnityEngine;

    public abstract class XMLViewer : ScriptableObject
    {
        public static readonly string CompName = nameof(XMLViewer);

        [Space(10)]
        [Button(nameof(Save), ButtonWidth = 300)]
        [SerializeField]
        private bool save;

        [Button(nameof(Load), ButtonWidth = 300)]
        [SerializeField]
        private bool load;

        [Header("Source")]
        [SerializeField]
        private string uri = "Example.xml";

        [Tooltip("The default path is Application.StreamingAssetsPath, if so, only file name needs to be specified in the uri. Full path otherwise.")]
        [SerializeField]
        private bool useDefaultPath = true;
        
        [ReadOnly]
        [SerializeField]
        private string defaultPath = Application.streamingAssetsPath;

        protected abstract void Save();

        protected abstract void Load();

        protected void SaveData<T>(T data)
        {
            string xmlFileUrl = useDefaultPath ? Path.Combine(defaultPath, uri) : uri;

            if (File.Exists(xmlFileUrl)) File.Delete(xmlFileUrl);

            // create the StreamingAsset folder if not exists
            // NOTE: Later in your app you want to use the StreamingAssets only 
            //       if your file shall be read-only!
            //       otherwise use persistentDataPath
            if (!Directory.Exists(Application.streamingAssetsPath)) Directory.CreateDirectory(Application.streamingAssetsPath);

            using (var stream = File.Open(xmlFileUrl, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                var serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(stream, data, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));
            }

#if UNITY_EDITOR
            // in the editor refresh the AssetDataBase of Unity 
            // so you see the added files in the Project View
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        protected void LoadData<T>(out T data)
        {
            string xmlFileUrl = useDefaultPath ? Path.Combine(defaultPath, uri) : uri;

            if (!File.Exists(xmlFileUrl))
            {
                Debug.LogWarning($"{CompName}: attempted to load '{xmlFileUrl}', but the file does not exist.");
                data = default(T);
                return;
            }

            using (var stream = File.Open(xmlFileUrl, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var serializer = new XmlSerializer(typeof(T));

                data = (T)serializer.Deserialize(stream);
            }
        }
    }
}
