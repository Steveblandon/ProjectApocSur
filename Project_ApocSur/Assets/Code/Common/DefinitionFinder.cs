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
        public static readonly string ClassName = nameof(DefinitionFinder);

        private const string directoryName = "Definitions";

        private static bool isInitialized;

        private static Action PostLoadCallbacks;

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

            PostLoadCallbacks?.Invoke();
            isInitialized = true;
        }

        private static void Add<T>(IEnumerable<T> defs, int count) where T : Def
        {
            DefinitionIndex<T>.Init(count).Add(defs);
            PostLoadCallbacks += DefinitionIndex<T>.Instance.PostLoadCallbacks;
        }

        public static bool TryFind<T>(string defName, out T def)
            where T : Def
        {
            if (DefinitionIndex<T>.Instance.TryFind(defName, out def))
            {
                return true;
            }
            else
            {
                LogUtility.Log(LogLevel.Warning, $"{ClassName}: failed to find '{defName}' of type [{typeof(T)}]");
                return false;
            }
        }

        private class DefinitionIndex<T> where T : Def
        {
            public static readonly string ClassName = nameof(DefinitionIndex<T>);
            public static DefinitionIndex<T> Instance = new DefinitionIndex<T>();
            private static Dictionary<string, T> index = new Dictionary<string, T>();
            public Action PostLoadCallbacks;

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
                        LogUtility.Log(LogLevel.Error, $"{ClassName}:<{typeof(T)}>.{nameof(def.Name)}='{def.Name}' is not valid. Skipping." );
                        continue;
                    }

                    /*// this is currently disabled so that cache can be overwritten via Unity inspector calls since 
                     * as long as Unity is running and this class hasn't been recompiled the cache stays untouched. In 
                     * general though it might be good to have this check in place to avoid unexpected behavior of what
                     * defs get uploaded... At the same time allowing duplicates would be useful for modding purposes.
                     * 
                    if (index.ContainsKey(def.Name))
                    {
                        LogUtility.Log(LogLevel.Warning, $"{ClassName}:<{typeof(T)}>.{nameof(def.Name)}='{def.Name}' duplicate found. Skipping.");
                        continue;
                    }*/

                    index[def.Name] = def;
                    this.PostLoadCallbacks += def.PostLoad;
                }
            }
        }

        [Serializable]
        [XmlRoot(ElementName = nameof(Defs))]
        public class Defs
        {
            [XmlElement(ElementName = nameof(StatDef))]
            public List<StatDef> statDefs = new List<StatDef>();

            [XmlElement(ElementName = nameof(StatModifierDef))]
            public List<StatModifierDef> statModDefs = new List<StatModifierDef>();

            [XmlElement(ElementName = nameof(InjuryDef))]
            public List<InjuryDef> injuryDefs = new List<InjuryDef>();

            [XmlElement(ElementName = nameof(BodyDef))]
            public List<BodyDef> bodyDefs = new List<BodyDef>();

            [XmlElement(ElementName = nameof(BodyPartDef))]
            public List<BodyPartDef> bodyPartDefs = new List<BodyPartDef>();
        }
    }
}
