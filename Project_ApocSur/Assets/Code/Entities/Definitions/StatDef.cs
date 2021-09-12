namespace Projapocsur.Entities.Definitions
{
    using System;

    [Serializable]
    public class StatDef : Def
    {
        public float DefaultValue;

        public static class NameOf
        {
            public static readonly string Height = nameof(Height);
        }
    }
}