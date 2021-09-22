namespace Projapocsur.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using Projapocsur.Common.Serialization;
    using ProjApocSur.Common;

    public class XmlSerializationTests : TestsBase
    {
        private string uri = Path.Combine(TestDataPath, "XmlSerializationTests.xml");

        [Test]
        public void TestXmlSerializer_Serialize_Exceptions()
        {
            using (var stream = File.Open(this.uri, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new List<int>()),
                    out XmlUnsupportedTypeException typeException);
                Assert.IsNotNull(typeException);

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new NonSerializableWithSerializableMembers()),
                    out typeException);
                Assert.IsNotNull(typeException);

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new SerializableWithUnsupportedType()),
                    out typeException);
                Assert.IsNotNull(typeException);

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new SerializableWithNestedUnsupportedType()),
                    out typeException);
                Assert.IsNotNull(typeException);

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new SerializableWithNoDefaultValueAndParamaterlessPublicConstructorType()),
                    out XmlInvalidException invalidException);
                Assert.IsNotNull(invalidException);
                Assert.IsTrue(invalidException.Message.Contains("instantiatable"));

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new SerializableWithCircularDependencySelf()),
                    out invalidException);
                Assert.IsNotNull(invalidException);
                Assert.IsTrue(invalidException.Message.Contains("circular dependency"));

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new SerializableWithCircularDependencyRef1()),
                    out invalidException);
                Assert.IsNotNull(invalidException);
                Assert.IsTrue(invalidException.Message.Contains("circular dependency"));

                ExceptionUtility.TryCatch(
                    () => XmlSerializer.Serialize(stream, new SerializableWithComplexStoredAsAttribute()),
                    out invalidException);
                Assert.IsNotNull(invalidException);
                Assert.IsTrue(invalidException.Message.Contains("attribute"));
            }
        }

        [Test]
        public void TestWriteAndRead_PrivateField_And_IgnoreUnmarked()
        {
            int valueToSerialize = 5;
            int valueToNotSerialize = 10;
            var objWithData = new SerializableWithPrivateMemberAndIgnoredField(valueToSerialize, valueToNotSerialize);
            Assert.AreEqual(valueToSerialize, objWithData.ValueToSerialize);
            Assert.AreEqual(valueToNotSerialize, objWithData.ValueToNotSerialize);

            ExceptionUtility.TryCatch(() => this.WriteToFile(objWithData), out Exception anyException);
            Assert.IsNull(anyException);

            var readObj = new SerializableWithPrivateMemberAndIgnoredField();
            ExceptionUtility.TryCatch(() => this.ReadFromFile(out readObj), out anyException);
            Assert.IsNull(anyException);

            Assert.AreEqual(valueToSerialize, readObj.ValueToSerialize);
            Assert.AreNotEqual(valueToNotSerialize, readObj.ValueToNotSerialize);
        }

        [Test]
        public void TestWriteAndRead_Field_And_Prop()
        {
            float expectedFieldValue = 5;
            string expectedPropValue = "burr";
            var objWithData = new SerializableWithFieldsAndProperties(expectedFieldValue, expectedPropValue);
            Assert.AreEqual(expectedFieldValue, objWithData.ValField);
            Assert.AreEqual(expectedPropValue, objWithData.ValProp);

            ExceptionUtility.TryCatch(() => this.WriteToFile(objWithData), out Exception anyException);
            Assert.IsNull(anyException);

            var readObj = new SerializableWithFieldsAndProperties();
            ExceptionUtility.TryCatch(() => this.ReadFromFile(out readObj), out anyException);
            Assert.IsNull(anyException);

            Assert.AreEqual(expectedFieldValue, readObj.ValField);
            Assert.IsTrue(expectedPropValue == readObj.ValProp);
        }

        [Test]
        public void TestWriteAndRead_NodeAttributes()
        {
            int expectedInt = 5;
            string expectedStr = "burr";
            var objWithData = new SerializableWithValuesStoredAsAttribute();
            objWithData.ValProp_int = expectedInt;
            objWithData.ValProp_str = expectedStr;
            Assert.AreEqual(expectedInt, objWithData.ValProp_int);
            Assert.IsTrue(expectedStr == objWithData.ValProp_str);

            ExceptionUtility.TryCatch(() => this.WriteToFile(objWithData), out Exception anyException);
            Assert.IsNull(anyException);

            var readObj = new SerializableWithValuesStoredAsAttribute();
            ExceptionUtility.TryCatch(() => this.ReadFromFile(out readObj), out anyException);
            Assert.IsNull(anyException);

            Assert.AreEqual(expectedInt, readObj.ValProp_int);
            Assert.IsTrue(expectedStr == readObj.ValProp_str);
        }

        [Test]
        public void TestWriteAndRead_ComplexType_And_PreferredNames()
        {
            float expectedFieldValue = 5;
            string expectedPropValue = "burr";
            var objWithData = new SerializableWithValuesStoredWithPreferredNames();
            objWithData.ValProp_str = expectedPropValue;
            objWithData.complexType = new SerializableWithFieldsAndProperties(expectedFieldValue, null);
            Assert.AreEqual(expectedFieldValue, objWithData.complexType.ValField);
            Assert.IsTrue(expectedPropValue == objWithData.ValProp_str);

            ExceptionUtility.TryCatch(() => this.WriteToFile(objWithData), out Exception anyException);
            Assert.IsNull(anyException);

            var readObj = new SerializableWithValuesStoredWithPreferredNames();
            ExceptionUtility.TryCatch(() => this.ReadFromFile(out readObj), out anyException);
            Assert.IsNull(anyException);

            Assert.AreEqual(expectedFieldValue, readObj.complexType.ValField);
            Assert.IsTrue(expectedPropValue == readObj.ValProp_str);
        }

        [Test]
        public void TestWriteAndRead_Lists()
        {
            var expectedintList = new List<int>() { 5 };
            var expectedlistOfLists = new List<List<List<string>>> { new List<List<string>>() { new List<string>() { "burr" } } };
            var expectedComplexTypeWithList = new SerializableWithList();
            expectedComplexTypeWithList.vals = expectedintList;
            var objWithData = new SerializableWithNestedLists();
            objWithData.wellNested = new List<SerializableWithList>() { expectedComplexTypeWithList };
            objWithData.refList = expectedComplexTypeWithList;
            objWithData.vals = expectedlistOfLists;

            ExceptionUtility.TryCatch(() => this.WriteToFile(objWithData), out Exception anyException);
            Assert.IsNull(anyException);

            var readObj = new SerializableWithNestedLists();
            ExceptionUtility.TryCatch(() => this.ReadFromFile(out readObj), out anyException);
            Assert.IsNull(anyException);

            Assert.IsNotNull(readObj.wellNested);
            Assert.IsNotNull(readObj.refList?.vals);
            Assert.IsNotNull(readObj.vals);

            Assert.AreEqual(objWithData.wellNested.Count, readObj.wellNested.Count);
            for (int i = 0; i < objWithData.wellNested.Count; i++)
            {
                Assert.IsNotNull(readObj.wellNested[i]?.vals);

                for (int j = 0; j < objWithData.wellNested.Count; j++)
                {
                    Assert.AreEqual(objWithData.wellNested[i].vals[j], readObj.wellNested[i].vals[j]);
                }
            }

            Assert.AreEqual(objWithData.refList.vals.Count, readObj.refList.vals.Count);
            for (int j = 0; j < objWithData.wellNested.Count; j++)
            {
                Assert.AreEqual(objWithData.refList.vals[j], readObj.refList.vals[j]);
            }

            Assert.AreEqual(objWithData.vals.Count, readObj.vals.Count);
            for (int j = 0; j < objWithData.wellNested.Count; j++)
            {
                Assert.AreEqual(objWithData.vals[j], readObj.vals[j]);
            }
        }

        [Test]
        public void TestWriteAndRead_inheritance()
        {
            int expectedInherritedPriv = 500;
            int expectedInherritedProt = 1000;
            var objWithData = new SerializableChild(expectedInherritedProt, expectedInherritedPriv);
            objWithData.mainVal = 5.3f;
            objWithData.inherritedVal = 100;
            objWithData.inherritedNoSerialize = 10;

            Assert.AreEqual(expectedInherritedPriv, objWithData.NonInherritedSerializableField);
            Assert.AreEqual(expectedInherritedProt, objWithData.InherritedVal_protected);

            ExceptionUtility.TryCatch(() => this.WriteToFile(objWithData), out Exception anyException);
            Assert.IsNull(anyException);

            var readObj = new SerializableChild();
            ExceptionUtility.TryCatch(() => this.ReadFromFile(out readObj), out anyException);
            Assert.IsNull(anyException);

            Assert.AreEqual(objWithData.mainVal, readObj.mainVal);
            Assert.AreEqual(objWithData.inherritedVal, readObj.inherritedVal);
            Assert.AreNotEqual(objWithData.inherritedNoSerialize, readObj.inherritedNoSerialize);
            Assert.AreNotEqual(objWithData.NonInherritedSerializableField, readObj.NonInherritedSerializableField);
            Assert.AreEqual(expectedInherritedProt, readObj.InherritedVal_protected);
        }

        private void WriteToFile(object data)
        {
            using (Stream stream = File.Open(this.uri, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                XmlSerializer.Serialize(stream, data);
            }
        }

        private void ReadFromFile<T>(out T data) where T : new()
        {
            using (var stream = File.Open(this.uri, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlDeserializer.Deserialize(stream, out data);
            }
        }
    }

    #region unserializable class examples
    public class NonSerializableWithSerializableMembers
    {
        [XmlMember]
        int value;
    }

    [XmlSerializable]
    public class SerializableWithUnsupportedType
    {
        [XmlMember]
        Dictionary<int, int> dict = new Dictionary<int, int>();
    }

    [XmlSerializable]
    public class SerializableWithNestedUnsupportedType
    {
        [XmlMember]
        SerializableWithUnsupportedType field = new SerializableWithUnsupportedType();
    }

    [XmlSerializable]
    public class SerializableWithNoDefaultValueAndParamaterlessPublicConstructorType
    {
        [XmlMember]
        SerializableWithoutParameterlessPublicConstructor val;
    }

    [XmlSerializable]
    public class SerializableWithoutParameterlessPublicConstructor
    {
        private SerializableWithoutParameterlessPublicConstructor() { }
    }

    [XmlSerializable]
    public class SerializableWithCircularDependencySelf
    {
        [XmlMember]
        SerializableWithCircularDependencySelf objRef;

        public SerializableWithCircularDependencySelf()
        {
            this.objRef = this;
        }
    }

    [XmlSerializable]
    public class SerializableWithCircularDependencyRef1
    {
        [XmlMember]
        SerializableWithCircularDependencyRef2 objRef;

        public SerializableWithCircularDependencyRef1()
        {
            this.objRef = new SerializableWithCircularDependencyRef2(this);
        }
    }

    [XmlSerializable]
    public class SerializableWithCircularDependencyRef2
    {
        [XmlMember]
        int value;

        [XmlMember]
        SerializableWithCircularDependencyRef1 objRef;

        public SerializableWithCircularDependencyRef2(SerializableWithCircularDependencyRef1 objRef)
        {
            this.objRef = objRef;
        }
    }

    [XmlSerializable]
    public class SerializableWithComplexStoredAsAttribute
    {
        [XmlMember(isAttribute: true)]
        public SerializableWithPrivateMemberAndIgnoredField value { get; set; }
    }
    #endregion

    [XmlSerializable]
    public class SerializableWithPrivateMemberAndIgnoredField
    {
        [XmlMember]
        private int value;

        private int value2;

        public SerializableWithPrivateMemberAndIgnoredField() { }

        public SerializableWithPrivateMemberAndIgnoredField(int valueToSerialize, int valueToNotSerialize)
        {
            this.value = valueToSerialize;
            this.value2 = valueToNotSerialize;
        }

        public int ValueToSerialize { get => this.value; }

        public int ValueToNotSerialize { get => this.value2; }
    }

    [XmlSerializable]
    public class SerializableWithFieldsAndProperties
    {
        [XmlMember]
        private float valField;

        [XmlMember]
        public string ValProp { get; private set; }

        public float ValField { get => this.valField; }

        public SerializableWithFieldsAndProperties() { }

        public SerializableWithFieldsAndProperties(float valField, string valProp)
        {
            this.valField = valField;
            this.ValProp = valProp;
        }
    }

    [XmlSerializable]
    public class SerializableWithValuesStoredAsAttribute
    {
        [XmlMember(isAttribute:true)]
        public string ValProp_str { get; set; }

        [XmlMember(isAttribute: true)]
        public int ValProp_int { get; set; }
    }

    [XmlSerializable]
    public class SerializableWithValuesStoredWithPreferredNames
    {
        [XmlMember(preferredName:"minbo", isAttribute:true)]
        public string ValProp_str { get; set; }

        [XmlMember(preferredName:"trueheart")]
        public SerializableWithFieldsAndProperties complexType { get; set; }
    }

    [XmlSerializable]
    public class SerializableWithList
    {
        [XmlMember]
        public List<int> vals { get; set; }
    }

    [XmlSerializable]
    public class SerializableWithNestedLists
    {
        [XmlMember]
        public List<List<List<string>>> vals;

        [XmlMember]
        public SerializableWithList refList;

        [XmlMember]
        public List<SerializableWithList> wellNested { get; set; }
    }

    [XmlSerializable]
    public class SerializableParent
    {
        [XmlMember]
        public int inherritedVal;

        public int inherritedNoSerialize;

        [XmlMember]
        protected int inherritedVal_protected;

        [XmlMember]
        private int nonInherritedSerializableField;

        public int NonInherritedSerializableField { get => this.nonInherritedSerializableField; }

        public int InherritedVal_protected { get => this.inherritedVal_protected; }

        public SerializableParent(int privval)
        {
            this.nonInherritedSerializableField = privval;
        }
    }

    [XmlSerializable]
    public class SerializableChild : SerializableParent
    {
        [XmlMember]
        public float mainVal;

        public SerializableChild() : base(0) { }

        public SerializableChild(int inherritedVal_protected, int parentint) 
            : base(parentint)
        {
            this.inherritedVal_protected = inherritedVal_protected;
        }
    }
}
