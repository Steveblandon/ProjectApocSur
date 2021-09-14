namespace Projapocsur.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public enum SeverityLevel
    {
        Minor,
        Major,
        Severe
    }

    [Serializable]
    public class Injury : Entity
    {
        
    }
}