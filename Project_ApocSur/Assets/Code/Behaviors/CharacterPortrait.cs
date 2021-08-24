namespace Projapocsur.Behaviors
{
    using Projapocsur.Common;
    using UnityEngine.EventSystems;

    public class CharacterPortrait : SimpleSelectable, ISelectHandler
    {
        public static readonly new string CompName = nameof(CharacterPortrait);

        private ImageColorSwitch imageColorSwitch;

        public void Start()
        {
            this.imageColorSwitch = Utils.GetSingleComponentInChildrenAndLogOnFailure<ImageColorSwitch>(this, CompName);
        }

        public void OnSelect(BaseEventData eventData)
        {
            this.OnSelect();
        }

        protected override void OnSelectInternal()
        {
            base.OnSelectInternal();

            if (imageColorSwitch != null)
            {
                imageColorSwitch.TurnOn();
            }

            CharacterManager.Instance.RegisterSelection(this);
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
