
namespace Projapocsur.Scripts
{
    using UnityEngine;

    /// <summary>
    /// A simple toggle to switch between colors.
    /// </summary>
    public class ColorSwitch : MonoBehaviour
    {
        [Tooltip("The color for when switch is on.")]
        [SerializeField]
        private Color onStateColor;

        /// <summary>
        /// True if switch is on, false otherwise.
        /// </summary>
        public bool IsOn { get; private set; }

        /// <summary>
        /// Color representing the state of the switch being on.
        /// </summary>
        public Color OnStateColor 
        { 
            get { return this.onStateColor; } 
            set { this.onStateColor = value; } 
        }

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