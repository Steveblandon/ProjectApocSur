namespace Projapocsur
{
    using System;

    public class ParentedProp<TValue, TParent> : Prop<TValue>, IParentedProp<TValue, TParent> where TValue : IComparable<TValue>
    {
        public new event Action<IParentedProp<TValue, TParent>> ValueChangedEvent;
        
        private TParent parent;

        public ParentedProp(TValue initialValue, TParent parent, Action OnValueChangePriorityCallback = null)
            : base (initialValue, OnValueChangePriorityCallback)
        {
            this.parent = parent;
        }

        public TParent Parent => this.parent;

        protected override void OnValueChange()
        {
            base.OnValueChange();

            this.ValueChangedEvent?.Invoke(this);
        }
    }
}