namespace Projapocsur.World
{
    using System;
    using Projapocsur.Serialization;

    [XmlSerializable]
    public class Stat : Thing<StatDef>
    {
        public event Action OnStateChangeEvent;
        public event Action OnValueAtMinEvent;
        public event Action OnValueAtMaxEvent;

        private bool useMinMaxLimiters;

        [XmlMember]
        private float minValue;

        [XmlMember]
        private float maxValue;

        public Stat() { }

        public Stat(string defName, float initialValue, float maxValue, float minValue = 0f) : base(defName)
        {
            this.Value = initialValue;
            this.DefaultValue = initialValue;
            this.MaxValue = maxValue;
            this.MinValue = minValue;
            this.useMinMaxLimiters = true;
            this.UpdateValueAsPercentage();
        }

        public Stat(string defName, float initialValue, bool useMinMaxLimiters = true) : this(defName, initialValue, initialValue)
        {
            this.useMinMaxLimiters = useMinMaxLimiters;
        }

        [XmlMember]
        public float Value { get; protected set; }

        [XmlMember]
        public float DefaultValue { get; protected set; }

        [XmlMember]
        public float ValueAsPercentage { get; protected set; }

        public float MinValue 
        { 
            get => this.minValue; 
            set
            {
                if (value <= this.maxValue)
                {
                    this.minValue = value;
                }

                this.InvokeEvents();
            }
        }

        public float MaxValue 
        { 
            get => this.maxValue; 
            set 
            {
                if (value >= minValue)
                {
                    this.maxValue = value;
                }

                this.InvokeEvents();
            }
        }

        public static Stat operator +(Stat stat, float amount)
        {
            stat.ApplyQuantity(amount);

            return stat;
        }

        public static Stat operator -(Stat stat, float amount)
        {
            stat.ApplyQuantity(-amount);

            return stat;
        }

        public static Stat operator *(Stat stat, float multiplier)
        {
            stat.ApplyMultiplier(multiplier);

            return stat;
        }

        public void ApplyQuantity(float amount)
        {
            if (this.useMinMaxLimiters)
            {
                float newValue = Math.Max(this.Value + amount, this.MinValue);      // we compare it to the min value incase of a negative amount
                this.Value = Math.Min(newValue, this.MaxValue);
                this.UpdateValueAsPercentage();
            }
            else
            {
                this.Value += amount;
            }

            this.InvokeEvents();
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

                this.UpdateValueAsPercentage();
            }
            else
            {
                this.Value *= multiplier;
            }

            this.InvokeEvents();
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

        private void UpdateValueAsPercentage()
        {
            float statValueSpan = Math.Abs(this.maxValue - this.minValue);
            float relativeStatValue = Math.Abs(this.Value - this.minValue);
            this.ValueAsPercentage = relativeStatValue / (statValueSpan == 0 ? 0.000001f : statValueSpan);
        }

        private void InvokeEvents()
        {
            this.OnStateChangeEvent?.Invoke();

            if (this.IsAtMaxValue())
            {
                this.OnValueAtMaxEvent?.Invoke();
            }
            else if (this.IsAtMinValue())
            {
                this.OnValueAtMinEvent?.Invoke();
            }
        }
    }
}