namespace Projapocsur.Scripts
{
    using Projapocsur.World;
    using UnityEngine;

    [RequireComponent(typeof(ToggleUI))]
    public class ShootButtonController : MonoBehaviour
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

        private void OnToggleStateChangeEvent(Selectable simpleSelectable)
        {
            if (this.toggleButton.IsSelected)
            {
                InputController.Main.RegisterCheckForTargetOnNextClick(this.OnTargetedClickEvent, skipFrameUpdate: true);    // NOTE: remember to set skipFrameUpdate to false once keyboard shortcuts are enabled... See parameter comments for further details.
                GameMaster.Instance.DraftTracker.OnDraftStateChangeEvent += this.OnDraftStateChangeEvent;
                this.OnDraftStateChangeEvent(GameMaster.Instance.DraftTracker.SelecteesDrafted);
            }
            else
            {
                this.CancelTargetingAndReset();
            }
        }

        private void OnDraftStateChangeEvent(bool? state)
        {
            if (GameMaster.Instance.DraftTracker.SelecteesDrafted == null || GameMaster.Instance.DraftTracker.SelecteesDrafted == false)
            {
                this.CancelTargetingAndReset();
            }
        }

        private void OnTargetedClickEvent(ITargetable target)
        {
            IDamageable damageable = target as IDamageable;

            if (damageable != null && GameMaster.Instance.DraftTracker.SelecteesDrafted == true)
            {
                foreach (var selectee in GameMaster.Instance.CharacterSelectionTracker.Selectees)
                {
                    selectee.EngageTarget(damageable, CombatEngagementMode.Shoot);
                }
            }

            this.CancelTargetingAndReset();
        }

        private void CancelTargetingAndReset()
        {
            InputController.Main.CancelCheckForTargetOnNextClick(this.OnTargetedClickEvent);
            GameMaster.Instance.DraftTracker.OnDraftStateChangeEvent -= this.OnDraftStateChangeEvent;

            if (this.toggleButton.IsSelected)
            {
                this.toggleButton.Toggle();
            }
        }
    }
}