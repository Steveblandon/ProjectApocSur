namespace Projapocsur.Entities
{
    using Projapocsur.Behaviors;
    using Projapocsur.Behaviors.UI;
    using Projapocsur.Common;

    public class PlayerCharacter
    {
        public static readonly string CompName = nameof(PlayerCharacter);

        public delegate void OnSelectStateChangeEventHandler(PlayerCharacter character);
        public event OnSelectStateChangeEventHandler OnSelectStateChangeEvent;

        private SimpleSelectable avatar;
        private SimpleSelectable portrait;

        public PlayerCharacter(SelectableWorldObject avatar, SelectableUI portrait)
        {
            ValidationUtils.ThrowIfNull(nameof(avatar), avatar);
            ValidationUtils.ThrowIfNull(nameof(portrait), portrait);

            this.avatar = avatar;
            this.portrait = portrait;
            this.avatar.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;
            this.portrait.OnSelectStateChangeEvent += this.OnCompSelectStateChangeEvent;

            EventManager.Instance.RegisterListener(EventType.WO_NothingClicked_Left, this.OnNoWorldObjectLeftClickedEvent);
        }

        public bool IsDrafted { get; private set; }
        public bool IsSelected { get; private set; }

        public void OnDraft()
        {
            this.IsDrafted = true;

            // reserving for future things to be handled when character is drafted.
        }

        public void OnUndraft()
        {
            this.IsDrafted = false;

            // reserving for future things to be handled when character is undrafted.
        }

        private void OnCompSelectStateChangeEvent(SimpleSelectable simpleSelectable)
        {
            if (this.IsSelected && !simpleSelectable.IsSelected)
            {
                this.IsSelected = false;
                avatar.OnDeselect();
                portrait.OnDeselect();
                this.OnSelectStateChangeEvent?.Invoke(this);
            }
            else if (!this.IsSelected && simpleSelectable.IsSelected)
            {
                this.IsSelected = true;
                avatar.OnSelect();
                portrait.OnSelect();
                this.OnSelectStateChangeEvent?.Invoke(this);
            }
        }

        private void OnNoWorldObjectLeftClickedEvent()
        {
            avatar.OnDeselect();
            portrait.OnDeselect();
        }
    }
}