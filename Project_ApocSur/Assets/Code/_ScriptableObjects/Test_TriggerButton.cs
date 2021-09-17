namespace Projapocsur.ScriptableObjects
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Projapocsur.Common.Serialization;
    using Projapocsur.Core;
    using Projapocsur.EditorAttributes;
    using Projapocsur.Entities.Definitions;
    using UnityEngine;

    [CreateAssetMenu]
    public class Test_TriggerButton : ScriptableObject
    {
        [Button(nameof(TestDefinitionFinder), ButtonWidth = 300)]
        [SerializeField]
        private bool testDefinitionFinder;
        private void TestDefinitionFinder()
        {
            DefinitionFinder.Init(true);
            DefinitionFinder.TryFind("HitPoints", out StatDef def);
            Debug.Log($"{def?.Name}: {def?.Description} found");
            DefinitionFinder.TryFind("Human", out BodyDef bodyDef);
            Debug.Log($"{bodyDef?.Name}: {bodyDef?.MaxHitPointsBase} found");
            DefinitionFinder.TryFind("Torso", out BodyPartDef bodyPartDef);
            Debug.Log($"{bodyPartDef?.Name}: {bodyPartDef?.MaxHitpointsBase} found");
            DefinitionFinder.TryFind("Laceration", out InjuryDef injuryDef);
            Debug.Log($"{injuryDef?.Name}: {injuryDef?.Description} found");
        }

        [SerializeField]
        private string fileToSerialize = "customSerializerTest.xml";
        [Button(nameof(TestCustomSerializer_Serialize), ButtonWidth = 300)]
        [SerializeField]
        private bool testCustomSerializer_Serialize;
        private void TestCustomSerializer_Serialize()
        {
            string filePath = Application.streamingAssetsPath;
            string uri = Path.Combine(filePath, fileToSerialize);

            if (File.Exists(uri))
            {
                File.Delete(uri);
            }
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            TestSerializableObject data = new TestSerializableObject();

            using (var stream = File.Open(uri, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                XmlSerializer.Serialize(stream, data);
            }
        }

        [SerializeField]
        private string fileToDeserialize = "customSerializerTest.xml";
        [Button(nameof(TestCustomSerializer_Deserialize), ButtonWidth = 300)]
        [SerializeField]
        private bool testCustomSerializer_Deserialize;
        private void TestCustomSerializer_Deserialize()
        {
            string filePath = Application.streamingAssetsPath;
            string uri = Path.Combine(filePath, fileToDeserialize);

            if (!File.Exists(uri))
            {
                Debug.LogWarning($"filepath doesn't exist [{uri}]");
                return;
            }

            using (var stream = File.Open(uri, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDeserializer.Deserialize(stream, out TestSerializableObject data);
                Debug.Log(data);
            }
        }

        [XmlSerializable]
        public class TestSerializableObject
        {
            private float nonSerializedField;

            [XmlMember(isAttribute: true)]
            private int valueFieldPri_int;

            [XmlMember(isAttribute: true)]
            private bool valueFieldPri_bool;

            [XmlMember(isAttribute: true)]
            private string valueFieldPri_str = string.Empty;

            [XmlMember]
            public int valueFieldPub_int;

            [XmlMember(isAttribute: true)]
            public float valuePropPub_float { get; set; }

            [XmlMember(preferredName:"miniNest")]
            public Mini NestedObject { get; private set; } = new Mini();

            [XmlMember]
            public IEnumerable<int> enumerable_generic_int { get; private set; } = new List<int>() { 5, 6 };

            [XmlMember]
            public IEnumerable enumerable { get; private set; } = new List<int>() { 4, 2 };

            /*[XmlMember]
            public DateTime Timestamp { get; private set; } = DateTime.UtcNow;*/

            public int nonSerializedProp { get; set; }

            /*[XmlMember]
            public TestSerializableObject NestedObjectCircularDep { get; private set; }*/

            /*public TestSerializableObject()
            {
                this.NestedObject = new Mini(this);
            }*/
        }

        [XmlSerializable]
        public class Mini
        {
            [XmlMember(preferredName: "supremeValue", isAttribute: true)]
            float value = 5;

            /*[XmlMember]
            public TestSerializableObject NestedObjectCircularDep { get; private set; }

            public Mini() { }

            public Mini(TestSerializableObject testSerializableObject)
            {
                this.NestedObjectCircularDep = testSerializableObject;
            }*/
        }
    }
}
