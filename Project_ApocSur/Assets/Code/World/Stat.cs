namespace Projapocsur.World
{
    using System;
    using Projapocsur.Common.Serialization;

    [XmlSerializable]
    public class Stat : Thing<StatDef>
    {
        public event Action OnStateChangeEvent;

        private bool useMinMaxLimiters;

        public Stat() { }

        public Stat(string defName, float initialValue, float maxValue, float minValue = 0f) : base(defName)
        {
            this.Value = initialValue;
            this.DefaultValue = initialValue;
            this.MaxValue = maxValue;
            this.MinValue = minValue;
            this.useMinMaxLimiters = true;
        }

        public Stat(string defName, float initialValue, bool useMinMaxLimiters = true) : this(defName, initialValue, initialValue)
        {
            this.useMinMaxLimiters = useMinMaxLimiters;
        }

        [XmlMember]
        public float Value { get; protected set; }

        [XmlMember]
        public float MinValue { get; protected set; }

        [XmlMember]
        public float MaxValue { get; protected set; }

        [XmlMember]
        public float DefaultValue { get; protected set; }

        public static Stat operator +(Stat stat, float amount)
        {
            stat.ApplyIncrease(amount);

            return stat;
        }

        public static Stat operator -(Stat stat, float amount)
        {
            stat.ApplyReduction(amount);

            return stat;
        }

        public static Stat operator *(Stat stat, float multiplier)
        {
            stat.ApplyMultiplier(multiplier);

            return stat;
        }

        public void ApplyReduction(float amount)
        {
            if (this.useMinMaxLimiters)
            {
                float newValue = this.Value - amount;
                this.Value = Math.Max(newValue, this.MinValue);
            }
            else
            {
                this.Value -= amount;
            }
        }

        public void ApplyIncrease(float amount)
        {
            if (this.useMinMaxLimiters)
            {
                float newValue = this.Value + amount;
                this.Value = Math.Min(newValue, this.MaxValue);
            }
            else
            {
                this.Value += amount;
            }
        }

        public void ApplyMultiplier(float multiplier)
        {
            if (this.useMinMaxLimiters)
            {
                float newValue = this.Value * multiplier;

                if (multiplier > 1f)
                {
                    this.Value = Math.Min(newValue, this.MaxValue);
                }
                else
                {
                    this.Value = Math.Max(newValue, this.MinValue);
                }
            }
            else
            {
                this.Value *= multiplier;
            }
        }

        public Stat Clone()
        {
            if (this.useMinMaxLimiters)
            {
                return new Stat(this.Def.Name, this.Value, this.MaxValue, this.MinValue);
            }
            else
            {
                return new Stat(this.Def.Name, this.Value, useMinMaxLimiters: false);
            }
        }

        public bool IsAtMinValue()
        {
            return this.Value == this.MinValue;
        }

        public bool IsAtMaxValue()
        {
            return this.Value == this.MaxValue;
        }
    }
}