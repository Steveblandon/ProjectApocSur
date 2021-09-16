namespace Projapocsur.Common
{
    using Projapocsur.Common.Utilities;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// This switches between its own preset color and the target <see cref="Image"/> color.
    /// </summary>
    public class ImageColorSwitch : ColorSwitch
    {
        private Image target;

        public ImageColorSwitch(Image image, Color onStateColor) : base(onStateColor, image.color)
        {
            ValidationUtility.ThrowIfNull(nameof(image), image);

            this.target = image;
        }

        public override void TurnOn()
        {
            base.TurnOn();

            this.target.color = this.OnStateColor;
        }

        public override void TurnOff()
        {
            base.TurnOff();

            this.target.color = this.OffStateColor;
        }
    }
}