namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;

    public class Character : SimpleSelectable
    {
        public static readonly new string CompName = nameof(Character);

        private SpriteRendererColorSwitch spriteRendererColorSwitch;

        public void Start()
        {
            this.spriteRendererColorSwitch = Utils.GetSingleComponentInChildrenAndLogOnFailure<SpriteRendererColorSwitch>(this, CompName);
        }

        protected override void OnSelectInternal()
        {
            base.OnSelectInternal();

            if (spriteRendererColorSwitch != null)
            {
                spriteRendererColorSwitch.TurnOn();
            }

            CharacterManager.Instance.RegisterSelection(this);
        }

        protected override void OnDeselectInternal()
        {
            base.OnDeselectInternal();

            if (spriteRendererColorSwitch != null)
            {
                spriteRendererColorSwitch.TurnOff();
            }
        }
    }
}