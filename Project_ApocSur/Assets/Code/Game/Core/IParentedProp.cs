namespace Projapocsur
{
    using System;

    public interface IParentedProp<TValue, TParent> : IProp<TValue> where TValue : IComparable<TValue>
    {
        new event Action<IParentedProp<TValue, TParent>> ValueChangedEvent;

        TParent Parent { get; }
    }
}