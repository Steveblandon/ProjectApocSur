namespace Projapocsur.ScriptableObjects
{
    using Projapocsur.Common.Utilities;
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
            StorageMode mode = useDefaultPath ? StorageMode.StreamingAssets : StorageMode.Absolute;
            StorageUtility.SaveData(data, uri, storageMode: mode);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh(); // refreshes AssetDataBase to make added files visible in Project View
#endif
        }

        protected void LoadData<T>(out T data)
        {
            StorageMode mode = useDefaultPath ? StorageMode.StreamingAssets : StorageMode.Absolute;
            StorageUtility.LoadData(out data, uri, storageMode: mode);
        }
    }
}
