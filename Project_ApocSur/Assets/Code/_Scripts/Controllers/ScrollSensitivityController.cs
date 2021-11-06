namespace Projapocsur.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(ScrollRect))]
    public class ScrollSensitivityController : MonoBehaviour
    {
        public const string CompName = nameof(ScrollSensitivityController);

        private IScrollViewContentItemManager scrollViewContentItemManager;
        private ScrollRect scrollRect;

        void Start()
        {
            scrollViewContentItemManager = this.GetComponentInChildren<IScrollViewContentItemManager>();

            if (!this.DisableOnMissingReference(this.scrollViewContentItemManager, nameof(this.scrollViewContentItemManager), CompName))
            {
                scrollRect = this.GetComponent<ScrollRect>();

                if (!this.DisableOnMissingReference(this.scrollRect, nameof(this.scrollRect), CompName))
                {
                    this.ContentItemHeightUpdatedEventHandler();    // update at least once to pick up initial value
                    this.scrollViewContentItemManager.ContentItemHeightUpdatedEvent += this.ContentItemHeightUpdatedEventHandler;
                }
            }
        }

        void OnDestroy()
        {
            if (this.scrollViewContentItemManager != null)
            {
                this.scrollViewContentItemManager.ContentItemHeightUpdatedEvent -= this.ContentItemHeightUpdatedEventHandler;
            }
        }

        private void ContentItemHeightUpdatedEventHandler()
        {
            scrollRect.scrollSensitivity = scrollViewContentItemManager.ContentItemHeight;
        }
    }
}
