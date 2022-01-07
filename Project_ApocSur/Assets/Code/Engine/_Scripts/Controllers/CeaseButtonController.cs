namespace Projapocsur.Engine
{
    using System;
    using Projapocsur.World;

    public class CeaseButtonController : PlayerCharacterEventToggleControllerBase
    {
        protected override Action<Character> TriggerEvent => character => character.Cease();
    }
}