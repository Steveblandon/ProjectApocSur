namespace Projapocsur.Behaviors
{
    using UnityEngine;

    /// <summary>
    /// The viewer serves as a reference to all canvas groups which can be triggered to be displayed/showed programmatically.
    /// The viewer will automatically display only one view/CanvasGroup at a time, and hide the previous one.
    /// </summary>
    public class CanvasGroupViewer : MonoBehaviour
    {
        public CanvasGroup CharacterSelectedView { get { return characterSelectedView; } }

        [Tooltip("Character selected view")]
        [SerializeField]
        private CanvasGroup characterSelectedView;

        private CanvasGroup currentActiveView;

        public void Show(CanvasGroup target)
        {
            if (target == this.currentActiveView)
            {
                return;
            }

            this.Hide(currentActiveView);

            target.alpha = 1;
            target.interactable = true;
            target.blocksRaycasts = true;
            this.currentActiveView = target;
        }

        public void Hide(CanvasGroup target)
        {
            if (target == null)
            {
                return;
            }

            target.alpha = 0;
            target.interactable = false;
            target.blocksRaycasts = false;
            
            if (target == this.currentActiveView)
            {
                this.currentActiveView = null;
            }
        }

        private void Start()
        {
            this.Hide(this.characterSelectedView);
        }
    }
}