namespace Projapocsur.UI
{
    using System;

    public interface ISelectHandler
    {
        /// <summary>
        /// </summary>
        /// <param name="noSelectionCallback"> 
        /// The callback for when the UI is disengaged (i.e. no UIElement is selected). Could be set to null if
        /// no callback is required.
        /// </param>
        void OnSelect(out Action noSelectionCallback);
    }
}