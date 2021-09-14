namespace Projapocsur.Entities.Definitions
{
    using System;

    [Serializable]
    public class StatModifierDef : Def
    {
        public DefRef<StatDef> AffectedStat;

        public override void PostLoad()
        {
            base.PostLoad();
            AffectedStat.PostLoad();
        }
    }
}