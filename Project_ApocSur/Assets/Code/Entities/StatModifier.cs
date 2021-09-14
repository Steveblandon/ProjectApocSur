namespace Projapocsur.Entities
{
    using System;
    using Projapocsur.Entities.Definitions;

    [Serializable]
    public class StatModifier : Entity
    {
        public StatModifierDefRef DefRef;
        public string ChangeLabel;

        public StatModifier() { }

        public StatModifier(StatModifierDefRef defRef)
        {
            this.DefRef = defRef;
        }

        public void Process(Stat stat)
        {

        }

        public override void PostLoad()
        {
            base.PostLoad();
            DefRef.PostLoad();
        }
    }
}