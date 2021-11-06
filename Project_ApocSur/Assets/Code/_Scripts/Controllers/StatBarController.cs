namespace Projapocsur.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class StatBarController : StatViewController
    {
        public const string CompName = nameof(StatBarController);

        [SerializeField]
        private Image image, background;

        [SerializeField]
        private bool enableBackground;

        [SerializeField]
        private Direction direction;

        private RectTransform rectTransform, imageObjRectTransform;

        private Color imageOriginalColor;

        // Start is called before the first frame update
        void Start()
        {
            if (!this.DisableOnMissingReference(image, nameof(image), CompName))
            {
                this.rectTransform = this.GetComponent<RectTransform>();
                this.imageObjRectTransform = this.image.GetComponent<RectTransform>();
                this.imageOriginalColor = this.image.color;
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
        }

        protected override void OnStatStateChangeEventHandler()
        {
            base.OnStatStateChangeEventHandler();

            float statPercentageReduction = this.GetStatPercentageReduction();
            image.color = this.GetColorWithNewHue(this.imageOriginalColor, statPercentageReduction);
            this.UpdateRectTransform(statPercentageReduction);
        }

        private void UpdateRectTransform(float percentage)
        {
            switch (direction)
            {
                case Direction.Up:
                    this.imageObjRectTransform.SetTop(this.rectTransform.rect.height * percentage);
                    this.imageObjRectTransform.SetBottom(0);
                    this.imageObjRectTransform.SetLeft(0);
                    this.imageObjRectTransform.SetRight(0);
                    break;
                case Direction.Down:
                    this.imageObjRectTransform.SetTop(0);
                    this.imageObjRectTransform.SetBottom(this.rectTransform.rect.height * percentage);
                    this.imageObjRectTransform.SetLeft(0);
                    this.imageObjRectTransform.SetRight(0);
                    break;
                case Direction.Left:
                    this.imageObjRectTransform.SetTop(0);
                    this.imageObjRectTransform.SetBottom(0);
                    this.imageObjRectTransform.SetLeft(this.rectTransform.rect.width * percentage);
                    this.imageObjRectTransform.SetRight(0);
                    break;
                case Direction.Right:
                    this.imageObjRectTransform.SetTop(0);
                    this.imageObjRectTransform.SetBottom(0);
                    this.imageObjRectTransform.SetLeft(0);
                    this.imageObjRectTransform.SetRight(this.rectTransform.rect.width * percentage);
                    break;
            }
        }
    }
}
