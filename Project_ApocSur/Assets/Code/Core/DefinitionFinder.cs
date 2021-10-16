namespace Projapocsur
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Common;
    using Projapocsur.World;

    /// <summary>
    /// Utility for finding definitions loaded into memory.
    /// </summary>
    public static class DefinitionFinder
    {
        public static readonly string ClassName = nameof(DefinitionFinder);
        private static bool isInitialized;
        private static Action postLoadCallbacks;
        private const string directoryName = "Definitions";

        public static void Init(string rootFilePath = null)
        {
            if (isInitialized)
            {
                return;
            }

            string[] filePaths = string.IsNullOrWhiteSpace(rootFilePath) ? 
                StorageUtility.GetDirectoryFiles(directoryName) : 
                StorageUtility.GetDirectoryFiles(directoryName, rootFilePath);

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

            postLoadCallbacks?.Invoke();
            isInitialized = true;
        }

        private static void Add<T>(IEnumerable<T> defs, int count) where T : Def
        {
            DefinitionIndex<T>.Init(count).Add(defs);
            postLoadCallbacks += DefinitionIndex<T>.Instance.postLoadCallbacks;
        }

        public static T Find<T>(string defName)
            where T : Def
        {
            if (DefinitionIndex<T>.Instance.TryFind(defName, out T def))
            {
                return def;
            }
            else
            {
                throw new KeyNotFoundException($"{ClassName}: failed to find '{defName}' of type [{typeof(T)}]");
            }
        }

        private class DefinitionIndex<T> where T : Def
        {
            public static readonly string ClassName = nameof(DefinitionIndex<T>);
            public static DefinitionIndex<T> Instance = new DefinitionIndex<T>();
            private static Dictionary<string, T> index = new Dictionary<string, T>();
            public Action postLoadCallbacks;

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
                    if (string.IsNullOrWhiteSpace(def?.Name))
                    {
                        LogUtility.Log(LogLevel.Error, $"{ClassName}:<{typeof(T)}>.{nameof(def.Name)}='{def.Name}' is not valid. Skipping.");
                        continue;
                    }

                    if (index.ContainsKey(def.Name))
                    {
                        LogUtility.Log(LogLevel.Warning, $"{ClassName}:<{typeof(T)}>.{nameof(def.Name)}='{def.Name}' duplicate detected. Skipping.");
                        continue;
                    }

                    index[def.Name] = def;
                    this.postLoadCallbacks += def.PostLoad;
                }
            }
        }

        [XmlSerializable]
        public class Defs
        {
            [XmlMember]
            public List<StatDef> statDefs;

            [XmlMember]
            public List<InjuryDef> injuryDefs;

            [XmlMember]
            public List<BodyDef> bodyDefs;

            [XmlMember]
            public List<BodyPartDef> bodyPartDefs;

            [XmlMember]
            public List<StanceDef> stanceDefs;
        }
    }
}
