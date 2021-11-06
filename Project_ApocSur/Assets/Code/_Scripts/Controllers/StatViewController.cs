namespace Projapocsur.Scripts
{
    using UnityEngine;
    using System;
    using Projapocsur.World;

    public abstract class StatViewController : MonoBehaviour
    {
        [SerializeField]
        private int maxHueDeviation;

        private Stat stat;

        public Stat Stat 
        {
            get => this.stat;
            set
            {
                if (value != this.stat)
                {
                    if (this.stat != null)
                    {
                        this.stat.OnStateChangeEvent -= this.OnStatStateChangeEventHandler;
                    }
                    
                    this.stat = value;
                    this.stat.OnStateChangeEvent += this.OnStatStateChangeEventHandler;
                    this.OnStatStateChangeEventHandler();
                }
            }
        }

        protected float GetStatPercentageReduction()
        {
            if (this.Stat == null)
            {
                Debug.LogError($"{nameof(GetStatPercentageReduction)} failed, no {nameof(this.Stat)} found");
            }

            float statValueSpan = Math.Abs(this.Stat.MaxValue - this.Stat.MinValue);
            float relativeStatValue = Math.Abs(this.Stat.Value - this.Stat.MinValue);
            return 1 - (relativeStatValue / (statValueSpan == 0 ? 0.000001f : statValueSpan));
        }

        protected Color GetColorWithNewHue(Color original, float percentageReduction)
        {
            if (this.maxHueDeviation <= 0f && original != null)
            {
                return original;
            }
            else
            {
                Color.RGBToHSV(original, out float H, out float S, out float V);
                float newHue = H - (this.maxHueDeviation / 360f * percentageReduction);
                return Color.HSVToRGB(newHue, S, V);
            }
        }

        protected virtual void OnStatStateChangeEventHandler()
        {
        }
    }
}
