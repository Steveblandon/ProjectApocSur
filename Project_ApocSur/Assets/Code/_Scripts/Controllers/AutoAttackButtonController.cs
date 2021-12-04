namespace Projapocsur.Scripts
{
    using System;
    using Projapocsur.World;

    public class AutoAttackButtonController : PlayerCharacterToggleControllerBase
    {
        protected override IProp<bool> TrackedProp => GameMaster.Instance.PlayerCharacterSelection.IsAutoAttackEnabled;

        protected override Func<Character, Prop<bool>> AffectedCharacterProp => character => character.IsAutoAttackEnabled;
    }
}