namespace Projapocsur.Scripts
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

            EventManager.Instance.RegisterListener(EventType.UI_NothingClicked, this.OnDeselect);
        }

        protected override void OnDeselectInternal()
        {
            base.OnDeselectInternal();

            if (imageColorSwitch != null)
            {
                imageColorSwitch.TurnOff();
            }

            EventManager.Instance.UnregisterListener(EventType.UI_NothingClicked, this.OnDeselect);
        }

        private void OnDisable()
        {
            EventManager.Instance.UnregisterListener(EventType.UI_NothingClicked, this.OnDeselect);
        }
    }
}
