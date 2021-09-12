namespace Projapocsur.Entities.Definitions
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Serializable]
    public class InjuryDef : Def
    {
        [XmlArray]
        [XmlArrayItem(ElementName = nameof(StatModifier))]
        public List<StatModifierDefRef> Effects = new List<StatModifierDefRef>();

        public override void PostLoad()
        {
            base.PostLoad();

            foreach (var effect in this.Effects)
            {
                effect.PostLoad();
            }
        }
    }
}
