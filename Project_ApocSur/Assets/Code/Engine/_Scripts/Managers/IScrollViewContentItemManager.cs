namespace Projapocsur.Engine
{
    using System;

    public interface IScrollViewContentItemManager
    {
        public event Action ContentItemHeightUpdatedEvent;

        public float ContentItemHeight { get; }
    }
}
