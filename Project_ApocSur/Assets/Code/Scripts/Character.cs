namespace Projapocsur
{
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