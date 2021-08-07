namespace Projapocsur.UI
{
    using System;

    public interface IMouseButtonUpHandler
    {
        /// <summary>
        /// </summary>
        /// <param name="callbackOnUIDisengage"> The callback for when the UI is disengaged (i.e. no UIElement is clicked)</param>
        void OnMouseButtonUp(out Action callbackOnUIDisengage);
    }
}
