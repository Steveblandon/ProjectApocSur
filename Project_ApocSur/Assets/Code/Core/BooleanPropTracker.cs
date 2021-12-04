namespace Projapocsur
{
    public class BooleanPropTracker
    {
        public BooleanPropTracker(bool initialValue)
        {
            this.Latest = new Prop<bool>(initialValue);
        }

        public Prop<bool> Latest { get; }

        public void Track(IProp<bool> prop) => prop.ValueChangedEvent += this.OnStateChangeEvent;

        public void Untrack(IProp<bool> prop) => prop.ValueChangedEvent -= this.OnStateChangeEvent;

        private void OnStateChangeEvent(IProp<bool> prop) => this.Latest.Value = prop.Value;
    }
}
