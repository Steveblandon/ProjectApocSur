namespace Projapocsur.Common
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// This switches between its own preset color and the target <see cref="Image"/> color.
    /// </summary>
    public class ImageColorSwitch : ColorSwitch
    {
        public static readonly string CompName = nameof(ImageColorSwitch);

        private Image target;

        public ImageColorSwitch(Image image, Color onStateColor) : base(onStateColor, image.color)
        {
            ValidationUtils.ThrowIfNull(nameof(image), image);

            this.target = image;
        }

        public override void TurnOn()
        {
            base.TurnOn();

            if (target != null)
            {
                target.color = this.OnStateColor;
            }
        }

        public override void TurnOff()
        {
            base.TurnOff();

            if (target != null)
            {
                target.color = this.OffStateColor;
            }
        }
    }
}