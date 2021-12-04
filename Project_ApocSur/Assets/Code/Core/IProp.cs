namespace Projapocsur
{
    using System;

    public interface IProp<TValue> where TValue : IComparable<TValue>
    {
        event Action<IProp<TValue>> ValueChangedEvent;

        TValue Value { get; }
    }
}