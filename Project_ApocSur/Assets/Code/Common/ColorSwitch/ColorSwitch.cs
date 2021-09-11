
namespace Projapocsur.Common
{
    using Projapocsur.Common.Utilities;
    using UnityEngine;

    /// <summary>
    /// A simple toggle to switch between two colors.
    /// </summary>
    public class ColorSwitch
    {
        public ColorSwitch(Color onStateColor, Color offStateColor)
        {
            ValidationUtils.ThrowIfNull(nameof(onStateColor), onStateColor);
            ValidationUtils.ThrowIfNull(nameof(offStateColor), offStateColor);

            this.OnStateColor = onStateColor;
            this.OffStateColor = offStateColor;
        }

        /// <summary>
        /// True if switch is on, false otherwise.
        /// </summary>
        public bool IsOn { get; private set; }

        /// <summary>
        /// Color representing the state of the switch being on.
        /// </summary>
        public Color OnStateColor { get; set; }

        /// <summary>
        /// Color representing the state of the switch being off.
        /// </summary>
        public Color OffStateColor { get; set; }

        /// <summary>
        /// Current color.
        /// </summary>
        public Color CurrentColor { get; private set; }

        /// <summary>
        /// Turn on the switch.
        /// </summary>
        public virtual void TurnOn()
        {
            this.IsOn = true;
            this.CurrentColor = this.OnStateColor;
        }

        /// <summary>
        /// Turn off the switch.
        /// </summary>
        public virtual void TurnOff()
        {
            this.IsOn = false;
            this.CurrentColor = this.OffStateColor;
        }
    }
}