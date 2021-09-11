namespace Projapocsur.Common
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Serialization;
    using Projapocsur.Common.Utilities;
    using Projapocsur.Entities.Definitions;

    /// <summary>
    /// Utility for finding definitions loaded into memory.
    /// </summary>
    public static class DefinitionFinder
    {
        private const string directoryName = "Definitions";

        private static bool isInitialized;

        public static void Init(bool reset = false)
        {
            if (isInitialized && !reset)
            {
                return;
            }

            string[] filePaths = StorageUtility.GetDirectoryFiles(directoryName);

            foreach (var filePath in filePaths)
            {
                StorageUtility.LoadData(out Defs data, filePath, StorageMode.Absolute);

                if (data == null)
                {
                    continue;
                }

                FieldInfo[] fields = data.GetType().GetFields();

                foreach (var field in fields)
                {
                    dynamic defs = field.GetValue(data);
                    if (defs.Count > 0)
                    {
                        Add(defs, defs.Count);
                    }
                }
            }

            isInitialized = true;
        }

        private static void Add<T>(IEnumerable<T> defs, int count) where T : Def
        {
            DefinitionIndex<T>.Init(count).Add(defs);
        }

        public static bool TryFind<T>(string defName, out T def)
            where T : Def
            => DefinitionIndex<T>.Instance.TryFind(defName, out def);

        private class DefinitionIndex<T> where T : Def
        {
            public static readonly string ClassName = nameof(DefinitionIndex<T>);
            public static DefinitionIndex<T> Instance = new DefinitionIndex<T>();
            private static Dictionary<string, T> index = new Dictionary<string, T>();
            
            private DefinitionIndex() { }

            public static DefinitionIndex<T> Init(int initialSize)
            {
                if (index.Count == 0)
                {
                    index = new Dictionary<string, T>(initialSize);
                }
                return Instance;
            }

            public bool TryFind(string defName, out T def) => index.TryGetValue(defName, out def);

            public void Add(IEnumerable<T> defs)
            {
                if (defs == null)
                {
                    return;
                }

                foreach (T def in defs)
                {
                    if (def?.Name == null)
                    {
                        throw new ArgumentNullException("name", $"Source: {ClassName}<{typeof(T)}>");
                    }

                    index[def.Name] = def;
                }
            }
        }

        [Serializable]
        [XmlRoot(ElementName = nameof(Defs))]
        public class Defs
        {
            [XmlElement(ElementName = nameof(StatDef))]
            public List<StatDef> statDefs = new List<StatDef>();

            [XmlElement(ElementName = nameof(BodyDef))]
            public List<BodyDef> bodyDefs = new List<BodyDef>();
        }
    }
}
