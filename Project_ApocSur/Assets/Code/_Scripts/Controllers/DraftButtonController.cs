namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(ToggleUI))]
    public class DraftButtonController : MonoBehaviour
    {
        public static readonly string CompName = nameof(DraftButtonController);

        private ToggleUI toggleButton;

        void Start()
        {
            this.toggleButton = this.GetComponent<ToggleUI>();
            this.toggleButton.OnSelectStateChangeEvent += this.OnToggleStateChangeEvent;
            IProp<bool> draftedProp = GameMaster.Instance.PlayerCharacterSelection.IsDrafted;
            draftedProp.ValueChangedEvent += this.OnDraftStateChangeEvent;
            this.OnDraftStateChangeEvent(draftedProp);
        }

        void OnDestroy()
        {
            this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            GameMaster.Instance.PlayerCharacterSelection.IsDrafted.ValueChangedEvent -= this.OnDraftStateChangeEvent;
        }

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            bool selecteesAreDrafted = GameMaster.Instance.PlayerCharacterSelection.IsDrafted.Value;

            if (this.toggleButton.IsSelected && !selecteesAreDrafted)
            {
                foreach (Character character in GameMaster.Instance.PlayerCharacterSelection.All)
                {
                    character.IsDrafted.Value = true;
                }
            }
            else if (!this.toggleButton.IsSelected && selecteesAreDrafted)
            {
                foreach (Character character in GameMaster.Instance.PlayerCharacterSelection.All)
                {
                    character.IsDrafted.Value = false;
                }
            }
        }

        private void OnDraftStateChangeEvent(IProp<bool> prop)
        {
            bool selecteesAreDrafted = GameMaster.Instance.PlayerCharacterSelection.IsDrafted.Value;

            if ((this.toggleButton.IsSelected && !selecteesAreDrafted) || (!this.toggleButton.IsSelected && selecteesAreDrafted))
            {
                this.toggleButton.Toggle();
            }
        }
    }
}