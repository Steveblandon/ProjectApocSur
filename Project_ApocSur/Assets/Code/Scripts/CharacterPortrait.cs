namespace Projapocsur
{
    public class CharacterPortrait : SimpleSelectable
    {
        public static readonly new string CompName = nameof(CharacterPortrait);

        private ImageColorSwitch imageColorSwitch;

        public void Start()
        {
            this.imageColorSwitch = Utils.GetSingleComponentInChildrenAndLogOnFailure<ImageColorSwitch>(this, CompName);
        }

        protected override void OnSelectInternal()
        {
            base.OnSelectInternal();

            if (imageColorSwitch != null)
            {
                imageColorSwitch.TurnOn();
            }
        }

        protected override void OnDeselectInternal()
        {
            base.OnDeselectInternal();

            if (imageColorSwitch != null)
            {
                imageColorSwitch.TurnOff();
            }
        }
    }
}
