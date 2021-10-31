namespace Projapocsur.Scripts
{
    using Projapocsur.Common;
    using UnityEngine;
    using UnityEngine.UI;

    public class StatBarUI : StatViewUI
    {
        [SerializeField]
        private Image image, background;

        [SerializeField]
        private bool enableBackground;

        [SerializeField]
        private Direction direction;

        private RectTransform rectTransform, rectTransformInternal;

        // Start is called before the first frame update
        void Start()
        {
            if (image == null)
            {
                Debug.LogError($"missing image reference. disabling {this.name}.");
                this.enabled = false;
            }
            else
            {
                this.rectTransform = this.GetComponent<RectTransform>();
                this.rectTransformInternal = this.image.GetComponent<RectTransform>();
                this.originalColor = this.image.color;
            }

            if (background != null)
            {
                background.enabled = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (background != null)
            {
                background.enabled = this.enableBackground;
            }

            float statPercentageReduction = this.GetStatPercentageReduction();
            image.color = this.GetCurrentColor(statPercentageReduction);
            this.UpdateRectTransform(statPercentageReduction);
        }

        void UpdateRectTransform(float percentage)
        {
            switch (direction)
            {
                case Direction.Up:
                    this.rectTransformInternal.SetTop(this.rectTransform.rect.height * percentage);
                    this.rectTransformInternal.SetBottom(0);
                    this.rectTransformInternal.SetLeft(0);
                    this.rectTransformInternal.SetRight(0);
                    break;
                case Direction.Down:
                    this.rectTransformInternal.SetTop(0);
                    this.rectTransformInternal.SetBottom(this.rectTransform.rect.height * percentage);
                    this.rectTransformInternal.SetLeft(0);
                    this.rectTransformInternal.SetRight(0);
                    break;
                case Direction.Left:
                    this.rectTransformInternal.SetTop(0);
                    this.rectTransformInternal.SetBottom(0);
                    this.rectTransformInternal.SetLeft(this.rectTransform.rect.width * percentage);
                    this.rectTransformInternal.SetRight(0);
                    break;
                case Direction.Right:
                    this.rectTransformInternal.SetTop(0);
                    this.rectTransformInternal.SetBottom(0);
                    this.rectTransformInternal.SetLeft(0);
                    this.rectTransformInternal.SetRight(this.rectTransform.rect.width * percentage);
                    break;
            }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}
