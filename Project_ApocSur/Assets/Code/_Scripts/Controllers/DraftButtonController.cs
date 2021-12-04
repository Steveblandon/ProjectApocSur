namespace Projapocsur.Scripts
{
    using System;
    using Projapocsur.World;

    public class DraftButtonController : PlayerCharacterToggleControllerBase
    {
        protected override IProp<bool> TrackedProp => GameMaster.Instance.PlayerCharacterSelection.IsDrafted;

        protected override Func<Character, Prop<bool>> AffectedCharacterProp => character => character.IsDrafted;
    }
}