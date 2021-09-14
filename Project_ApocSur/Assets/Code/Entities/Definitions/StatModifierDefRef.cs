namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public class StatModifierDefRef : DefRef<StatModifierDef>
    {
        [XmlAttribute]
        public float Change;

        [XmlAttribute]
        public float Multiplier = 1;
    }

    public static class StatModifierDefRefExtensions
    {
        public static void PostLoad(this List<StatModifierDefRef> defRefs) => defRefs.ForEach((defRef) => defRef.PostLoad());
    }
}