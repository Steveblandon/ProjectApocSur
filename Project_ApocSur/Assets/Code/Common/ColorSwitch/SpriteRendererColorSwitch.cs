namespace Projapocsur.Common
{
    using Projapocsur.Common.Utility;
    using UnityEngine;

    /// <summary>
    /// This switches between its own preset color and the target <see cref="SpriteRenderer"/> color.
    /// </summary>
    public class SpriteRendererColorSwitch : ColorSwitch
    {
        private SpriteRenderer target;

        public SpriteRendererColorSwitch(SpriteRenderer renderer, Color onStateColor) : base(onStateColor, renderer.color)
        {
            ValidationUtils.ThrowIfNull(nameof(renderer), renderer);

            this.target = renderer;
        }

        public override void TurnOn()
        {
            base.TurnOn();

            this.target.color = this.CurrentColor;
        }

        public override void TurnOff()
        {
            base.TurnOff();

            this.target.color = this.CurrentColor;
        }
    }
}