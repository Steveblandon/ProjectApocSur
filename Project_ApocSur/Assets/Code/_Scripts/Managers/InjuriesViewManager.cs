namespace Projapocsur.Scripts
{
    using System;
    using System.Collections.Generic;
    using Projapocsur.World;
    using UnityEngine;

    public class InjuriesViewManager : MonoBehaviour, IScrollViewContentItemManager
    {
        public const string CompName = nameof(InjuriesViewManager);

        public event Action ContentItemHeightUpdatedEvent;

        [SerializeField]
        private GameObject injuryViewPrefab;

        private RectTransform rectTransform;

        private List<InjuryViewController> injuryViewControllers = new List<InjuryViewController>();

        private float contentItemHeight;

        public float ContentItemHeight 
        {
            get => contentItemHeight;
            private set
            {
                if (value != this.contentItemHeight)
                {
                    this.contentItemHeight = value;
                    this.ContentItemHeightUpdatedEvent?.Invoke();
                }
            }
        }

        void OnEnable()
        {
            this.rectTransform ??= this.GetComponent<RectTransform>();
            this.DisableOnMissingReference(this.rectTransform, nameof(this.rectTransform), CompName);
            this.DisableOnMissingReference(this.injuryViewPrefab, nameof(this.injuryViewPrefab), CompName);
        }

        public void ManageViewsFor(IReadOnlyCollection<Injury> injuries)
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            // clear existing controllers
            this.injuryViewControllers.ForEach(controller => this.DestroyController(controller));
            this.injuryViewControllers.Clear();

            // create new controllers
            foreach (var injury in injuries)
            {
                this.AddToExistingManagedViews(injury, resizeToFit: false);
            }

            this.ResizeToFitViews();
        }

        public void AddToExistingManagedViews(Injury injury, bool resizeToFit = true)
        {
            GameObject injuryView = Instantiate(injuryViewPrefab, this.transform);

            if (this.UpdateViewHeight(injuryView, this.injuryViewControllers.Count) != null
                && injuryView.TryGetComponent(out InjuryViewController controller))
            {
                this.injuryViewControllers.Add(controller);
                controller.Injury = injury;
                controller.InjuryIsHealedEvent += this.InjuryIsHealedEventHandler;
            }
            else
            {
                Debug.LogError($"unable to find {nameof(InjuryViewController)} component in newly instantiated {nameof(injuryView)}");
            }

            if (resizeToFit)
            {
                this.ResizeToFitViews();
            }
        }

        private void InjuryIsHealedEventHandler(InjuryViewController controller)
        {
            this.injuryViewControllers.Remove(controller);
            this.DestroyController(controller);
            this.UpdateViewHeights();
        }

        private void DestroyController(InjuryViewController controller)
        {
            controller.InjuryIsHealedEvent -= this.InjuryIsHealedEventHandler;
            Destroy(controller.gameObject);
        }

        private void UpdateViewHeights()
        {
            for (int index = 0; index < this.injuryViewControllers.Count; index++)
            {
                this.UpdateViewHeight(this.injuryViewControllers[index].gameObject, index);
            }

            this.ResizeToFitViews();
        }

        private RectTransform UpdateViewHeight(GameObject injuryView, int index)
        {
            if (injuryView.TryGetComponent(out RectTransform injuryViewRectTransform))
            {
                this.ContentItemHeight = injuryViewRectTransform.rect.height;
                var y = (this.ContentItemHeight / -2) - (this.ContentItemHeight * index);
                injuryViewRectTransform.localPosition = new Vector3(injuryViewRectTransform.localPosition.x, y, injuryViewRectTransform.localPosition.z);
            }
            else
            {
                Debug.LogError($"unable to find {nameof(RectTransform)} component in newly instantiated {nameof(injuryView)}");
            }

            return injuryViewRectTransform;
        }

        private void ResizeToFitViews()
        {
            float newHeight = this.ContentItemHeight * this.injuryViewControllers.Count;
            this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x, newHeight);
        }
    }
}
