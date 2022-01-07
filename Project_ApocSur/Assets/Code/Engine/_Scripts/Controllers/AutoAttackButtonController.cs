namespace Projapocsur.Engine
{
    using System;
    using Projapocsur.World;

    public class AutoAttackButtonController : CharacterPropToggleControllerBase
    {
        protected override IProp<bool> TrackedProp => GameMaster.Instance.PlayerCharacterSelection.IsAutoAttackEnabled;

        protected override Func<Character, Prop<bool>> AffectedCharacterProp => character => character.IsAutoAttackEnabled;
    }
}