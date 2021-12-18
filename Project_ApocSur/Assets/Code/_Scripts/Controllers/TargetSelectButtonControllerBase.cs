namespace Projapocsur.Scripts
{
    using UnityEngine;

    /// <summary>
    /// Allows target selection only when currently selected characters are drafted.
    /// </summary>
    [RequireComponent(typeof(ToggleUI))]
    public abstract class TargetSelectButtonControllerBase : MonoBehaviour
    {
        private ToggleUI toggleButton;

        void Start()
        {
            this.toggleButton = this.GetComponent<ToggleUI>();
            this.toggleButton.OnSelectStateChangeEvent += this.OnToggleStateChangeEvent;
        }

        void OnDestroy()
        {
            this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            this.CancelTargetingAndReset();
        }

        protected abstract void OnTargetedClickEvent(ITargetable target);

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.toggleButton.IsSelected)
            {
                InputController.Main.RegisterCheckForTargetOnNextClick(this.OnTargetedClickEvent, skipFrameUpdate: true);    // NOTE: remember to set skipFrameUpdate to false once keyboard shortcuts are enabled... See parameter comments for further details.
                IProp<bool> draftedProp = GameMaster.Instance.PlayerCharacterSelection.IsDrafted;
                draftedProp.ValueChangedEvent += this.OnDraftStateChangeEvent;
                this.OnDraftStateChangeEvent(draftedProp);
            }
            else
            {
                this.CancelTargetingAndReset();
            }
        }

        private void OnDraftStateChangeEvent(IProp<bool> prop)
        {
            bool selecteesAreDrafted = GameMaster.Instance.PlayerCharacterSelection.IsDrafted.Value;

            if (!selecteesAreDrafted)
            {
                this.CancelTargetingAndReset();
            }
        }

        protected void CancelTargetingAndReset()
        {
            InputController.Main.CancelCheckForTargetOnNextClick(this.OnTargetedClickEvent);
            GameMaster.Instance.PlayerCharacterSelection.IsDrafted.ValueChangedEvent -= this.OnDraftStateChangeEvent;

            if (this.toggleButton.IsSelected)
            {
                this.toggleButton.Toggle();
            }
        }
    }
}