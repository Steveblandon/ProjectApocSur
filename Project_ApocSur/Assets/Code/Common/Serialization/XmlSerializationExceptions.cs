namespace Projapocsur.Common.Serialization
{
    using System;

    /// <summary>
    /// Exception to standarize exceptions thrown from processing XML.
    /// </summary>
    public class XmlSerializationException : Exception
    {
        public XmlSerializationException(string message, Exception innerException = null) : base(message, innerException) { }
    }

    /// <inheritdoc/>
    public class XmlUnsupportedTypeException : XmlSerializationException
    {
        public XmlUnsupportedTypeException(Type type, string paramName, string source = null)
            : base($"unsupported type [param: name={paramName}, type={type} source={source ?? string.Empty}]") { }
    }

    /// <inheritdoc/>
    public class XmlInvalidException : XmlSerializationException
    {
        public XmlInvalidException(string message, Type type, string paramName, string paramValue = null, string source = null, Exception innerException = null)
            : base($"{message ?? string.Empty} [parentClass:{source ?? "<unknown>"} [param: name={paramName}, value={paramValue ?? "<omitted>"}, type={type?.Name ?? "<unknown>"}]]", innerException) { }

    }
}