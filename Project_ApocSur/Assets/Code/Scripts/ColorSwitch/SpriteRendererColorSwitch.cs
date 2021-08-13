namespace Projapocsur.Scripts
{
    using UnityEngine;

    /// <summary>
    /// This component switches between its own preset color and it's game object <see cref="SpriteRenderer"/> component color.
    /// </summary>
    public class SpriteRendererColorSwitch : ColorSwitch
    {
        public static readonly string CompName = nameof(SpriteRendererColorSwitch);

        private SpriteRenderer spriteRenderer;

        public void Start()
        {
            if (this.TryGetComponent(out this.spriteRenderer))
            {
                this.OffStateColor = spriteRenderer.color;
            }
            else
            {
                Debug.LogWarning($"{CompName}: no {nameof(this.spriteRenderer)} comp detected on {this.name}. disabling component...");
                this.enabled = false;
            }
        }

        public override void TurnOn()
        {
            base.TurnOn();

            if (this.enabled && spriteRenderer != null)
            {
                spriteRenderer.color = this.OnStateColor;
            }
            else
            {
                Debug.Log($"{CompName}: attempted to turn ON switch for {this.name}, but either no {nameof(this.spriteRenderer)}  is available or comp is disabled.");
            }
        }

        public override void TurnOff()
        {
            base.TurnOff();

            if (this.enabled && spriteRenderer != null)
            {
                spriteRenderer.color = this.OffStateColor;
            }
            else
            {
                Debug.Log($"{CompName}: attempted to turn OFF switch for {this.name}, but either no {nameof(this.spriteRenderer)}  is available or comp is disabled.");
            }
        }
    }
}