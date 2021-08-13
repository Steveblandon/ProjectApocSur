namespace Projapocsur.Scripts
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

            EventManager.Instance.RegisterListener(EventType.WO_NothingClicked, this.OnDeselect);
        }

        protected override void OnDeselectInternal()
        {
            base.OnDeselectInternal();

            if (spriteRendererColorSwitch != null)
            {
                spriteRendererColorSwitch.TurnOff();
            }

            EventManager.Instance.UnregisterListener(EventType.WO_NothingClicked, this.OnDeselect);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterListener(EventType.WO_NothingClicked, this.OnDeselect);
        }
    }
}