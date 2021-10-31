namespace Projapocsur.Scripts
{
    using UnityEngine;
    using System;

    public class StatViewUI : MonoBehaviour
    {
        protected Color originalColor;

        [SerializeField]
        private float statMinValue, statMaxValue, statValue;

        [SerializeField]
        private int maxHueDeviation;

        protected float GetStatPercentageReduction()
        {
            // stat changes will be event-driven and the responsible for triggering a rect transform update,
            // but for the time being call from here with the use of the dummy stats.
            float statValueSpan = Math.Abs(statMaxValue - statMinValue);
            float relativeStatValue = Math.Abs(statValue - statMinValue);
            return 1 - (relativeStatValue / (statValueSpan == 0 ? 0.000001f : statValueSpan));
        }

        protected Color GetCurrentColor(float percentageReduction)
        {
            if (this.maxHueDeviation <= 0f && this.originalColor != null)
            {
                return this.originalColor;
            }
            else
            {
                Color.RGBToHSV(this.originalColor, out float H, out float S, out float V);
                float newHue = H - (this.maxHueDeviation / 360f * percentageReduction);
                return Color.HSVToRGB(newHue, S, V);
            }
        }
    }
}
