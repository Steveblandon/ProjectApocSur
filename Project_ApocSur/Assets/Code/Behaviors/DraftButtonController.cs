namespace Projapocsur.Behaviors
{
    using Projapocsur.Behaviors.UI;
    using Projapocsur.Common;
    using UnityEngine;
    using EventType = Common.EventType;

    [RequireComponent(typeof(ToggleUI))]
    public class DraftButtonController : MonoBehaviour
    {
        public static readonly string CompName = nameof(DraftButtonController);

        private ToggleUI toggleButton;

        private void Start()
        {
            this.toggleButton = this.GetComponent<ToggleUI>();
            this.toggleButton.OnSelectStateChangeEvent += this.OnToggleStateChangeEvent;
            EventManager.Instance.RegisterListener(EventType.CM_CharacterDraftStateChanged, this.OnDraftStateChangeEvent);
        }

        private void OnDestroy()
        {
            this.toggleButton.OnSelectStateChangeEvent -= this.OnToggleStateChangeEvent;
            EventManager.Instance.UnregisterListener(EventType.CM_CharacterDraftStateChanged, this.OnDraftStateChangeEvent);
        }

        private void OnToggleStateChangeEvent(SimpleSelectable simpleSelectable)
        {
            if (toggleButton.IsSelected && DraftTracker.Instance.SelecteesDrafted == false)
            {
                DraftTracker.Instance.DraftSelectees();
            }
            else if (!toggleButton.IsSelected && DraftTracker.Instance.SelecteesDrafted == true)
            {
                DraftTracker.Instance.UndraftSelectees();
            }
        }

        private void OnDraftStateChangeEvent()
        {
            if ((toggleButton.IsSelected && (DraftTracker.Instance.SelecteesDrafted == false || DraftTracker.Instance.SelecteesDrafted == null))
                || (!toggleButton.IsSelected && DraftTracker.Instance.SelecteesDrafted == true))
            {
                toggleButton.Toggle();
            }
        }
    }
}