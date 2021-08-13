namespace Projapocsur.Scripts
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// This component switches between its own preset color and it's game object <see cref="Image"/> component color.
    /// </summary>
    public class ImageColorSwitch : ColorSwitch
    {
        public static readonly string CompName = nameof(ImageColorSwitch);

        private Image image;

        public void Start()
        {
            if (this.TryGetComponent(out this.image))
            {
                this.OffStateColor = image.color;
            }
            else
            {
                Debug.LogWarning($"{CompName}: no {nameof(this.image)} comp detected on {this.name}. disabling component...");
                this.enabled = false;
            }
        }

        public override void TurnOn()
        {
            base.TurnOn();

            if (this.enabled && image != null)
            {
                image.color = this.OnStateColor;
            }
            else
            {
                Debug.Log($"{CompName}: attempted to turn ON switch for {this.name}, but either no {nameof(this.image)} is available or comp is disabled.");
            }
        }

        public override void TurnOff()
        {
            base.TurnOff();

            if (this.enabled && image != null)
            {
                image.color = this.OffStateColor;
            }
            else
            {
                Debug.Log($"{CompName}: attempted to turn OFF switch for {this.name}, but either no {nameof(this.image)} is available or comp is disabled.");
            }
        }
    }
}