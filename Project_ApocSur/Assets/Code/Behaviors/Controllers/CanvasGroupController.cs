namespace Projapocsur.Behaviors.Controllers
{
    using Projapocsur.Common;
    using UnityEngine;
    using EventType = Common.EventType;

    /// <summary>
    /// A central place to coordinate and control what Canvas Group should be displayed in a event-driven manner.
    /// </summary>
    [RequireComponent(typeof(CanvasGroupViewer))]
    public class CanvasGroupController : MonoBehaviour
    {
        private CanvasGroupViewer viewer;

        private void Start()
        {
            viewer = this.GetComponent<CanvasGroupViewer>();

            EventManager.Instance.RegisterListener(EventType.CM_CharacterSelectionStateChanged, this.OnCharacterSelectionStateChangeEvent);
        }

        private void OnDestroy()
        {
            EventManager.Instance.UnregisterListener(EventType.CM_CharacterSelectionStateChanged, this.OnCharacterSelectionStateChangeEvent);
        }

        private void OnCharacterSelectionStateChangeEvent()
        {
            if (DraftTracker.Instance.SelecteesDrafted != null)
            {
                this.viewer.Show(this.viewer.CharacterSelectedView);
            }
            else
            {
                this.viewer.Hide(this.viewer.CharacterSelectedView);
            }
        }
    }
}