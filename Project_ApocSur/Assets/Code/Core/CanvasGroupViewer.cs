namespace Projapocsur.Core
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
            if (target == currentActiveView)
            {
                return;
            }

            CanvasGroup targetTopParentGroup = GetTopParentGroup(target);

            if (targetTopParentGroup != currentActiveViewTopParent)
            {
                Hide(currentActiveView);
            }

            target.alpha = 1;
            target.interactable = true;
            target.blocksRaycasts = true;
            currentActiveView = target;
            currentActiveViewTopParent = targetTopParentGroup;
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

            if (target == currentActiveView)
            {
                currentActiveView = null;
                currentActiveViewTopParent = null;
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
                return GetTopParentGroup(parentGroup);
            }
        }
    }
}