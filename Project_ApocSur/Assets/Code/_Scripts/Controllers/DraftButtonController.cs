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
            GameMaster.Instance.DraftTracker.DraftStateChangedEvent += this.OnDraftStateChangeEvent;
            this.OnDraftStateChangeEvent(GameMaster.Instance.DraftTracker.SelecteesDrafted);
        }

        void OnDestroy()
        {
            this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            GameMaster.Instance.DraftTracker.DraftStateChangedEvent -= this.OnDraftStateChangeEvent;
        }

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.toggleButton.IsSelected && GameMaster.Instance.DraftTracker.SelecteesDrafted == false)
            {
                foreach (Character character in GameMaster.Instance.CharacterSelectionTracker.Selectees)
                {
                    character.IsDrafted = true;
                }
            }
            else if (!this.toggleButton.IsSelected && GameMaster.Instance.DraftTracker.SelecteesDrafted == true)
            {
                foreach (Character character in GameMaster.Instance.CharacterSelectionTracker.Selectees)
                {
                    character.IsDrafted = false;
                }
            }
        }

        private void OnDraftStateChangeEvent(bool? state)
        {
            if (this.toggleButton.IsSelected && (GameMaster.Instance.DraftTracker.SelecteesDrafted == false || GameMaster.Instance.DraftTracker.SelecteesDrafted == null)
                || !this.toggleButton.IsSelected && GameMaster.Instance.DraftTracker.SelecteesDrafted == true)
            {
                this.toggleButton.Toggle();
            }
        }
    }
}