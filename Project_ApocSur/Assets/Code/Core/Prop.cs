namespace Projapocsur
{
    using System;

    public class Prop<TValue> : IProp<TValue> where TValue : IComparable<TValue>
    {
        public event Action<IProp<TValue>> ValueChangedEvent;

        private TValue value;
        private readonly Action ValueChangedPriorityCallback;

        public Prop(TValue initialValue, Action OnValueChangePriorityCallback = null)
        {
            this.value = initialValue;
            this.ValueChangedPriorityCallback = OnValueChangePriorityCallback;
        }

        public TValue Value
        {
            get => this.value;
            set
            {
                if (this.value.CompareTo(value) == 0)
                {
                    return;
                }

                this.value = value;
                this.OnValueChange();
            }
        }

        protected virtual void OnValueChange()
        {
            this.ValueChangedPriorityCallback?.Invoke();
            this.ValueChangedEvent?.Invoke(this);
        }
    }
}