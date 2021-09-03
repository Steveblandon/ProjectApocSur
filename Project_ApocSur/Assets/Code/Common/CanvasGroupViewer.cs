namespace Projapocsur.Common
{
    using UnityEngine;

    /// <summary>
    /// The viewer helps with controlling what canvasGroup should be displayed/hidden while respecting view hierarchy. 
    /// </summary>
    public class CanvasGroupViewer
    {
        public static CanvasGroupViewer Instance { get { return _instance; } }
        private static CanvasGroupViewer _instance = new CanvasGroupViewer();

        private CanvasGroup currentActiveView;
        private CanvasGroup currentActiveViewTopParent;

        public void Show(CanvasGroup target)
        {
            if (target == this.currentActiveView)
            {
                return;
            }

            CanvasGroup targetTopParentGroup = this.GetTopParentGroup(target);

            if (targetTopParentGroup != this.currentActiveViewTopParent)
            {
                this.Hide(this.currentActiveView);
            }

            target.alpha = 1;
            target.interactable = true;
            target.blocksRaycasts = true;
            this.currentActiveView = target;
            this.currentActiveViewTopParent = targetTopParentGroup;
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
                this.currentActiveViewTopParent = null;
            }
        }

        private CanvasGroup GetTopParentGroup(CanvasGroup target)
        {
            if (target == null)
            {
                return null;
            }

            CanvasGroup parentGroup = target.transform.parent?.GetComponent<CanvasGroup>();

            if (parentGroup == null || parentGroup == target)
            {
                return target;
            }
            else
            {
                return this.GetTopParentGroup(parentGroup);
            }
        }
    }
}