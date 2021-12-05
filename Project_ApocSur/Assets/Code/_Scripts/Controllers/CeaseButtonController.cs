namespace Projapocsur.Scripts
{
    using System;
    using Projapocsur.World;

    public class CeaseButtonController : PlayerCharacterEventToggleControllerBase
    {
        protected override Action<Character> TriggerEvent => character => character.Cease();
    }
}