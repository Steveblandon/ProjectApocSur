namespace Projapocsur.Common.Serialization
{
    using System;

    /// <summary>
    /// Indicates a class can be serialized by the <see cref="XmlSerializer"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class XmlSerializableAttribute : Attribute { }
}
